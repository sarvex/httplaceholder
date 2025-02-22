<template>
  <input
    type="file"
    name="file"
    ref="uploadField"
    @change="loadTextFromFile"
    :multiple="multiple"
  />
  <button :class="buttonClasses" @click="uploadClick">
    {{ buttonText }}
  </button>
</template>

<script lang="ts">
import { defineComponent, type PropType, ref } from "vue";
import type { FileUploadedModel } from "@/domain/file-uploaded-model";
import { UploadButtonType } from "@/domain/upload-button-type";

export default defineComponent({
  name: "UploadButton",
  props: {
    buttonText: {
      type: String,
      required: true,
    },
    multiple: {
      type: Boolean,
      default: null,
      required: false,
    },
    buttonClasses: {
      type: String,
      default: "btn btn-primary me-2",
      required: false,
    },
    resultType: {
      type: String as PropType<UploadButtonType>,
      default: UploadButtonType.Text,
    },
  },
  setup(props, { emit }) {
    // Refs
    const uploadField = ref<HTMLElement>();

    // Methods
    const uploadClick = function () {
      if (uploadField.value) {
        uploadField.value.click();
      }
    };
    const loadTextFromFile = (ev: any) => {
      const files: File[] = Array.from(ev.target.files);
      for (const file of files) {
        const reader = new FileReader();
        reader.onload = (e: any) => {
          const uploadedFile: FileUploadedModel = {
            filename: file.name,
            result: e.target.result,
          };
          emit("uploaded", uploadedFile);
        };
        switch (props.resultType) {
          case UploadButtonType.Text:
            reader.readAsText(file);
            break;
          case UploadButtonType.Base64:
            reader.readAsDataURL(file);
            break;
          default:
            throw `Result type for upload not supported: ${props.resultType}`;
        }
      }
    };

    return {
      uploadField,
      uploadClick,
      loadTextFromFile,
    };
  },
});
</script>

<style scoped>
input[type="file"] {
  display: none;
}
</style>
