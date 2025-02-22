import { defineStore } from "pinia";
import { del, get, post, put } from "@/utils/api";
import yaml from "js-yaml";
import type { FullStubModel } from "@/domain/stub/full-stub-model";
import type { FullStubOverviewModel } from "@/domain/stub/full-stub-overview-model";
import { useStubFormStore } from "@/store/stubForm";

const stubFormStore = useStubFormStore();

export interface UpdateStubInputModel {
  input: string;
  stubId: string;
}

export interface CreateStubBasedOnRequestInputModel {
  correlationId: string;
  doNotCreateStub?: boolean;
}

export const useStubsStore = defineStore({
  id: "stubs",
  state: () => ({}),
  getters: {},
  actions: {
    getStub(stubId: string): Promise<FullStubModel> {
      return get(`/ph-api/stubs/${stubId}`)
        .then((response) => Promise.resolve(response))
        .catch((error) => Promise.reject(error));
    },
    getStubsOverview(): Promise<FullStubOverviewModel[]> {
      return get("/ph-api/stubs/overview")
        .then((response) => Promise.resolve(response))
        .catch((error) => Promise.reject(error));
    },
    getStubs(): Promise<FullStubModel[]> {
      return get("/ph-api/stubs")
        .then((response) => Promise.resolve(response))
        .catch((error) => Promise.reject(error));
    },
    async flipEnabled(stubId: string): Promise<boolean> {
      const stub = (await this.getStub(stubId)).stub;
      stub.enabled = !stub.enabled;
      await put(`/ph-api/stubs/${stubId}`, stub);
      return stub.enabled;
    },
    async enableStub(stubId: string): Promise<boolean> {
      const stub = (await this.getStub(stubId)).stub;
      stub.enabled = true;
      await put(`/ph-api/stubs/${stubId}`, stub);
      return stub.enabled;
    },
    async disableStub(stubId: string): Promise<boolean> {
      const stub = (await this.getStub(stubId)).stub;
      stub.enabled = false;
      await put(`/ph-api/stubs/${stubId}`, stub);
      return stub.enabled;
    },
    deleteStub(stubId: string): Promise<any> {
      return del(`/ph-api/stubs/${stubId}`)
        .then((response) => Promise.resolve(response))
        .catch((error) => Promise.reject(error));
    },
    deleteStubs(): Promise<any> {
      return del("/ph-api/stubs")
        .then((response) => Promise.resolve(response))
        .catch((error) => Promise.reject(error));
    },
    addStubs(input: string): Promise<FullStubModel[]> {
      const parsedObject = yaml.load(input);
      const stubsArray = Array.isArray(parsedObject)
        ? parsedObject
        : [parsedObject];
      return post("/ph-api/stubs/multiple", stubsArray)
        .then((response) => {
          stubFormStore.setFormIsDirty(false);
          return Promise.resolve(response);
        })
        .catch((error) => Promise.reject(error));
    },
    updateStub(payload: UpdateStubInputModel): Promise<any> {
      const parsedObject = yaml.load(payload.input);
      return put(`/ph-api/stubs/${payload.stubId}`, parsedObject)
        .then((response) => {
          stubFormStore.setFormIsDirty(false);
          return Promise.resolve(response);
        })
        .catch((error) => Promise.reject(error));
    },
    createStubBasedOnRequest(
      payload: CreateStubBasedOnRequestInputModel
    ): Promise<FullStubModel> {
      return post(`/ph-api/requests/${payload.correlationId}/stubs`, {
        doNotCreateStub:
          payload.doNotCreateStub !== null ? payload.doNotCreateStub : false,
      })
        .then((response) => Promise.resolve(response))
        .catch((error) => Promise.reject(error));
    },
  },
});
