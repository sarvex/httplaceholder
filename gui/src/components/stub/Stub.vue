<template>
  <accordion-item @buttonClicked="showDetails" :opened="accordionOpened">
    <template v-slot:button-text>
      <span :class="{ disabled: !enabled }">
        {{ id }}
      </span>
      <span v-if="!enabled" class="disabled">&nbsp;(disabled)</span>
      <span v-if="overviewStub.metadata.readOnly" title="Stub is read-only"
        >&nbsp;<i class="bi-eye"></i
      ></span>
    </template>
    <template v-slot:accordion-body>
      <div v-if="fullStub">
        <div class="row mb-3">
          <div class="col-md-12">
            <router-link
              class="btn btn-success btn-sm me-2 btn-mobile"
              title="View all requests made for this stub"
              :to="{
                name: 'Requests',
                query: { filter: id },
              }"
              >Requests
            </router-link>
            <button
              class="btn btn-success btn-sm me-2 btn-mobile"
              title="Duplicate this stub"
              @click="duplicate"
            >
              Duplicate
            </button>
            <router-link
              v-if="!isReadOnly"
              class="btn btn-success btn-sm me-2 btn-mobile"
              title="Update this stub"
              :to="{
                name: 'StubForm',
                params: { stubId: id },
              }"
              >Update
            </router-link>
            <button
              v-if="!isReadOnly"
              class="btn btn-success btn-sm me-2 btn-mobile"
              :title="enableDisableTitle"
              @click="enableOrDisable"
            >
              {{ enableDisableText }}
            </button>
            <button
              class="btn btn-success btn-sm me-2 btn-mobile"
              @click="downloadStub"
            >
              Download
            </button>
            <router-link
              v-if="hasScenario"
              class="btn btn-success btn-sm me-2 btn-mobile"
              :to="{ name: 'ScenarioForm', params: { scenario: scenario } }"
              >Set scenario
            </router-link>
            <button
              v-if="!isReadOnly"
              class="btn btn-danger btn-sm me-2 btn-mobile"
              title="Delete the stub"
              @click="showDeleteModal = true"
            >
              Delete
            </button>
            <modal
              v-if="!isReadOnly"
              :title="deleteStubTitle"
              bodyText="The stub can't be recovered."
              :yes-click-function="deleteStub"
              :show-modal="showDeleteModal"
              @close="showDeleteModal = false"
            />
          </div>
        </div>
        <code-highlight language="yaml" :code="stubYaml" />
      </div>
    </template>
  </accordion-item>
</template>

<script lang="ts">
import { computed, type PropType, ref } from "vue";
import yaml from "js-yaml";
import { resources } from "@/constants/resources";
import { setIntermediateStub } from "@/utils/session";
import { useRouter } from "vue-router";
import dayjs from "dayjs";
import { handleHttpError } from "@/utils/error";
import { success } from "@/utils/toast";
import { useStubsStore } from "@/store/stubs";
import { defineComponent } from "vue";
import type { FullStubOverviewModel } from "@/domain/stub/full-stub-overview-model";
import type { FullStubModel } from "@/domain/stub/full-stub-model";
import { downloadBlob } from "@/utils/download";
import { vsprintf } from "sprintf-js";

export default defineComponent({
  name: "Stub",
  props: {
    overviewStub: {
      type: Object as PropType<FullStubOverviewModel>,
      required: true,
    },
  },
  setup(props, { emit }) {
    const stubStore = useStubsStore();
    const router = useRouter();

    // Functions
    const getStubId = () => props.overviewStub.stub.id;
    const isEnabled = () => props.overviewStub.stub.enabled;

    // Data
    const overviewStubValue = ref(props.overviewStub);
    const fullStub = ref<FullStubModel>();
    const showDeleteModal = ref(false);
    const accordionOpened = ref(false);

    // Computed
    const stubYaml = computed(() => {
      if (!fullStub.value) {
        return "";
      }

      return yaml.dump(fullStub.value.stub);
    });
    const scenario = computed(() => {
      if (!fullStub.value) {
        return null;
      }

      return fullStub.value.stub.scenario;
    });
    const hasScenario = computed(() => {
      return !!scenario.value;
    });
    const isReadOnly = computed(() =>
      fullStub.value ? fullStub.value.metadata.readOnly : true
    );
    const enableDisableTitle = computed(
      () => `${isEnabled() ? "Disable" : "Enable"} stub`
    );
    const enableDisableText = computed(() =>
      isEnabled() ? "Disable" : "Enable"
    );
    const enabled = computed(() => isEnabled());
    const deleteStubTitle = computed(() => `Delete stub '${getStubId()}'?`);
    const id = computed(() => overviewStubValue.value.stub.id);

    // Methods
    const showDetails = async () => {
      if (!fullStub.value) {
        try {
          fullStub.value = await stubStore.getStub(getStubId());

          // Sadly, when doing this without the timeout, it does the slide down incorrect.
          setTimeout(() => (accordionOpened.value = true), 1);
        } catch (e) {
          handleHttpError(e);
        }
      } else {
        accordionOpened.value = !accordionOpened.value;
      }
    };
    const duplicate = async () => {
      if (fullStub.value && fullStub.value.stub) {
        const stub = fullStub.value.stub;
        stub.id = `${stub.id}_${dayjs().format("YYYY-MM-DD_HH-mm-ss")}`;
        setIntermediateStub(yaml.dump(stub));
        await router.push({ name: "StubForm" });
      }
    };
    const enableOrDisable = async () => {
      if (fullStub.value) {
        try {
          const enabled = await stubStore.flipEnabled(getStubId());
          fullStub.value.stub.enabled = enabled;
          overviewStubValue.value.stub.enabled = enabled;
          let message;
          if (enabled) {
            message = vsprintf(resources.stubEnabledSuccessfully, [
              getStubId(),
            ]);
          } else {
            message = vsprintf(resources.stubDisabledSuccessfully, [
              getStubId(),
            ]);
          }

          success(message);
        } catch (e) {
          handleHttpError(e);
        }
      }
    };
    const deleteStub = async () => {
      try {
        await stubStore.deleteStub(getStubId());
        success(resources.stubDeletedSuccessfully);
        showDeleteModal.value = false;
        emit("deleted");
      } catch (e) {
        handleHttpError(e);
      }
    };
    const downloadStub = async () => {
      try {
        const fullStub = await stubStore.getStub(getStubId());
        const stub = fullStub.stub;
        const downloadString = `${resources.downloadStubsHeader}\n${yaml.dump(
          stub
        )}`;
        downloadBlob(`${stub.id}-stub.yml`, downloadString);
      } catch (e) {
        handleHttpError(e);
      }
    };

    return {
      showDetails,
      fullStub,
      stubYaml,
      duplicate,
      isReadOnly,
      enableOrDisable,
      enableDisableTitle,
      enableDisableText,
      overviewStubValue,
      deleteStub,
      deleteStubTitle,
      showDeleteModal,
      id,
      enabled,
      accordionOpened,
      hasScenario,
      scenario,
      downloadStub,
    };
  },
});
</script>

<style scoped>
.disabled {
  color: #969696;
}
</style>
