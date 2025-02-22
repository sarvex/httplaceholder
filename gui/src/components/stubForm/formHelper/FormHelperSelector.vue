<template>
  <div class="row mt-3" v-if="!showFormHelperItems">
    <div class="col-md-12">
      <button class="btn btn-outline-primary" @click="openFormHelperList">
        Add request / response value
      </button>
    </div>
  </div>
  <div class="row mt-3" v-if="showFormHelperItems">
    <div class="col-md-12">
      <div class="mb-3">
        <button
          class="btn btn-danger btn-mobile full-width"
          @click="closeFormHelperAndList"
        >
          Close list
        </button>
      </div>
      <div class="input-group mb-3">
        <input
          type="text"
          class="form-control"
          placeholder="Filter form helpers (press 'Escape' to close)..."
          v-model="formHelperFilter"
          ref="formHelperFilterInput"
        />
      </div>
      <div class="list-group">
        <template v-for="(item, index) in filteredStubFormHelpers" :key="index">
          <div v-if="item.isMainItem" class="list-group-item fw-bold fs-3">
            {{ item.title }}
          </div>
          <button
            v-else
            class="list-group-item list-group-item-action"
            @click="onFormHelperItemClick(item)"
          >
            <label class="fw-bold">{{ item.title }}</label>
            <span class="subtitle">{{ item.subTitle }}</span>
          </button>
        </template>
      </div>
    </div>
  </div>
  <div v-if="currentSelectedFormHelper" class="row mt-3">
    <div class="col-md-12">
      <div class="card">
        <div class="card-body">
          <ExampleSelector
            v-if="currentSelectedFormHelper === FormHelperKey.Example"
          />
          <HttpMethodSelector
            v-if="currentSelectedFormHelper === FormHelperKey.HttpMethod"
          />
          <TenantSelector
            v-if="currentSelectedFormHelper === FormHelperKey.Tenant"
          />
          <HttpStatusCodeSelector
            v-if="currentSelectedFormHelper === FormHelperKey.StatusCode"
          />
          <ResponseBodyHelper
            v-if="currentSelectedFormHelper === FormHelperKey.ResponseBody"
          />
          <ResponseBodyHelper
            v-if="
              currentSelectedFormHelper === FormHelperKey.ResponseBodyPlainText
            "
            :preset-response-body-type="ResponseBodyType.text"
          />
          <ResponseBodyHelper
            v-if="currentSelectedFormHelper === FormHelperKey.ResponseBodyJson"
            :preset-response-body-type="ResponseBodyType.json"
          />
          <ResponseBodyHelper
            v-if="currentSelectedFormHelper === FormHelperKey.ResponseBodyXml"
            :preset-response-body-type="ResponseBodyType.xml"
          />
          <ResponseBodyHelper
            v-if="currentSelectedFormHelper === FormHelperKey.ResponseBodyHtml"
            :preset-response-body-type="ResponseBodyType.html"
          />
          <ResponseBodyHelper
            v-if="
              currentSelectedFormHelper === FormHelperKey.ResponseBodyBase64
            "
            :preset-response-body-type="ResponseBodyType.base64"
          />
          <RedirectSelector
            v-if="currentSelectedFormHelper === FormHelperKey.Redirect"
          />
          <LineEndingSelector
            v-if="currentSelectedFormHelper === FormHelperKey.LineEndings"
          />
          <ScenarioSelector
            v-if="currentSelectedFormHelper === FormHelperKey.Scenario"
          />
          <SetDynamicMode
            v-if="currentSelectedFormHelper === FormHelperKey.DynamicMode"
          />
          <SetPath v-if="currentSelectedFormHelper === FormHelperKey.Path" />
          <SetFullPath
            v-if="currentSelectedFormHelper === FormHelperKey.FullPath"
          />
          <SetQuery v-if="currentSelectedFormHelper === FormHelperKey.Query" />
          <SetBody v-if="currentSelectedFormHelper === FormHelperKey.Body" />
          <SetHeader
            v-if="currentSelectedFormHelper === FormHelperKey.Header"
          />
          <SetForm v-if="currentSelectedFormHelper === FormHelperKey.Form" />
          <SetHost v-if="currentSelectedFormHelper === FormHelperKey.Host" />
        </div>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import { computed, onMounted, onUnmounted, ref, watch } from "vue";
import HttpMethodSelector from "@/components/stubForm/formHelper/HttpMethodSelector.vue";
import TenantSelector from "@/components/stubForm/formHelper/TenantSelector.vue";
import HttpStatusCodeSelector from "@/components/stubForm/formHelper/HttpStatusCodeSelector.vue";
import ResponseBodyHelper from "@/components/stubForm/formHelper/ResponseBodyHelper.vue";
import RedirectSelector from "@/components/stubForm/formHelper/RedirectSelector.vue";
import LineEndingSelector from "@/components/stubForm/formHelper/LineEndingSelector.vue";
import ScenarioSelector from "@/components/stubForm/formHelper/ScenarioSelector.vue";
import SetDynamicMode from "@/components/stubForm/formHelper/SetDynamicMode.vue";
import SetPath from "@/components/stubForm/formHelper/SetPath.vue";
import SetFullPath from "@/components/stubForm/formHelper/SetFullPath.vue";
import { useRoute } from "vue-router";
import { escapePressed } from "@/utils/event";
import { useStubFormStore } from "@/store/stubForm";
import { defineComponent } from "vue";
import {
  type StubFormHelper,
  stubFormHelpers,
} from "@/domain/stubForm/stub-form-helpers";
import { FormHelperKey } from "@/domain/stubForm/form-helper-key";
import { ResponseBodyType } from "@/domain/stubForm/response-body-type";
import ExampleSelector from "@/components/stubForm/formHelper/ExampleSelector.vue";
import SetQuery from "@/components/stubForm/formHelper/SetQuery.vue";
import SetBody from "@/components/stubForm/formHelper/SetBody.vue";
import SetHeader from "@/components/stubForm/formHelper/SetHeader.vue";
import SetForm from "@/components/stubForm/formHelper/SetForm.vue";
import SetHost from "@/components/stubForm/formHelper/SetHost.vue";

export default defineComponent({
  name: "FormHelperSelector",
  components: {
    SetHost,
    SetForm,
    SetHeader,
    SetBody,
    SetQuery,
    SetFullPath,
    ExampleSelector,
    LineEndingSelector,
    RedirectSelector,
    ResponseBodyHelper,
    HttpStatusCodeSelector,
    TenantSelector,
    HttpMethodSelector,
    ScenarioSelector,
    SetDynamicMode,
    SetPath,
  },
  setup() {
    const stubFormStore = useStubFormStore();
    const route = useRoute();

    // Refs
    const formHelperFilterInput = ref<HTMLElement>();

    // Data
    const showFormHelperItems = ref(false);
    const formHelperItems = ref();

    // Methods
    const onFormHelperItemClick = (item: StubFormHelper) => {
      if (item.defaultValueMutation) {
        item.defaultValueMutation(stubFormStore);
        stubFormStore.closeFormHelper();
      } else if (item.formHelperToOpen) {
        stubFormStore.openFormHelper(item.formHelperToOpen);
      }

      showFormHelperItems.value = false;
      formHelperFilter.value = "";
    };
    const openFormHelperList = () => {
      showFormHelperItems.value = true;
      setTimeout(() => {
        if (formHelperFilterInput.value) {
          formHelperFilterInput.value.focus();
        }
      }, 10);
    };
    const closeFormHelperAndList = () => {
      formHelperFilter.value = "";
      stubFormStore.closeFormHelper();
      showFormHelperItems.value = false;
    };

    // Computed
    const currentSelectedFormHelper = computed(
      () => stubFormStore.getCurrentSelectedFormHelper
    );
    const filteredStubFormHelpers = computed(() => {
      if (!formHelperFilter.value) {
        return stubFormHelpers;
      }
      return stubFormHelpers.filter((h) => {
        if (h.isMainItem) {
          return true;
        }

        return h.title
          .toLowerCase()
          .includes(formHelperFilter.value.toLowerCase());
      });
    });
    const formHelperFilter = computed({
      get: () => stubFormStore.getFormHelperSelectorFilter,
      set: (value) => stubFormStore.setFormHelperSelectorFilter(value),
    });

    // Watch
    watch(currentSelectedFormHelper, (formHelper) => {
      if (!formHelper) {
        showFormHelperItems.value = false;
      }
    });
    watch(
      () => route.params,
      () => closeFormHelperAndList()
    );

    // Lifecycle
    const escapeListener = (e: KeyboardEvent) => {
      if (escapePressed(e)) {
        e.preventDefault();
        closeFormHelperAndList();
      }
    };
    onMounted(() => document.addEventListener("keydown", escapeListener));
    onUnmounted(() => document.removeEventListener("keydown", escapeListener));

    return {
      formHelperItems,
      currentSelectedFormHelper,
      showFormHelperItems,
      filteredStubFormHelpers,
      onFormHelperItemClick,
      formHelperFilter,
      formHelperFilterInput,
      openFormHelperList,
      closeFormHelperAndList,
      FormHelperKey,
      ResponseBodyType,
    };
  },
});
</script>

<style scoped>
label {
  display: block;
  cursor: pointer;
}

.subtitle {
  font-size: 0.9em;
}
</style>
