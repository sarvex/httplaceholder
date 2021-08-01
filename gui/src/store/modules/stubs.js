import createInstance from "@/axios/axiosInstanceFactory";
import yaml from "js-yaml";

const state = () => ({});

const actions = {
  getStubsOverview() {
    return new Promise((resolve, reject) =>
      createInstance()
        .get("ph-api/stubs/overview")
        .then(response => resolve(response.data))
        .catch(error => reject(error))
    );
  },
  getStubs() {
    return new Promise((resolve, reject) =>
      createInstance()
        .get("ph-api/stubs")
        .then(response => resolve(response.data))
        .catch(error => reject(error))
    );
  },
  /* eslint no-empty-pattern: 0 */
  getStub({}, payload) {
    return new Promise((resolve, reject) =>
      createInstance()
        .get(`ph-api/stubs/${payload.stubId}`)
        .then(response => resolve(response.data))
        .catch(error => reject(error))
    );
  },
  /* eslint no-empty-pattern: 0 */
  deleteStub({}, payload) {
    return new Promise((resolve, reject) =>
      createInstance()
        .delete(`ph-api/stubs/${payload.stubId}`)
        .then(() => resolve())
        .catch(error => reject(error))
    );
  },
  deleteAllStubs() {
    return new Promise((resolve, reject) =>
      createInstance()
        .delete("ph-api/stubs")
        .then(() => resolve())
        .catch(error => reject(error))
    );
  },
  /* eslint no-empty-pattern: 0 */
  addStubs({}, payload) {
    return new Promise((resolve, reject) => {
      let stubsArray;
      let parsedObject;
      if (payload.inputIsJson) {
        parsedObject = payload.input;
      } else {
        try {
          parsedObject = yaml.safeLoad(payload.input);
        } catch (error) {
          reject(error.message);
          return;
        }
      }

      if (!Array.isArray(parsedObject)) {
        stubsArray = [parsedObject];
      } else {
        stubsArray = parsedObject;
      }

      createInstance()
        .post("ph-api/stubs/multiple", stubsArray)
        .then(r => r.data)
        .then(result => resolve(result))
        .catch(error => reject(error));
    });
  },
  /* eslint no-empty-pattern: 0 */
  createStubBasedOnRequest({}, payload) {
    return new Promise((resolve, reject) =>
      createInstance()
        .post(`ph-api/requests/${payload.correlationId}/stubs`, {
          doNotCreateStub:
            payload.doNotCreateStub !== null ? payload.doNotCreateStub : false
        })
        .then(response => resolve(response.data))
        .catch(error => reject(error))
    );
  },
  async enableStub({}, stubId) {
    const stub = (await createInstance().get(`ph-api/stubs/${stubId}`)).data
      .stub;
    stub.enabled = true;
    return new Promise((resolve, reject) =>
      createInstance()
        .put(`ph-api/stubs/${stubId}`, stub)
        .then(() => resolve())
        .catch(error => reject(error))
    );
  },
  async disableStub({}, stubId) {
    const stub = (await createInstance().get(`ph-api/stubs/${stubId}`)).data
      .stub;
    stub.enabled = false;
    return new Promise((resolve, reject) =>
      createInstance()
        .put(`ph-api/stubs/${stubId}`, stub)
        .then(() => resolve())
        .catch(error => reject(error))
    );
  }
};

const mutations = {};

const getters = {};

export default {
  namespaced: true,
  state,
  getters,
  mutations,
  actions
};
