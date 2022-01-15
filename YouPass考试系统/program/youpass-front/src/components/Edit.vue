<template>
  <div class="edit-component" @click.self="handleCancel">
    <div
      class="
        edit-container
        col-xxl-8
        offset-xxl-2
        col-xl-8
        offset-xl-2
        col-lg-8
        offset-lg-2
        col-sm-10
        offset-sm-1
      "
    >
      <h1>Edit</h1>
      <div class="edit-stage">
        <div class="left">
          <h2>Edit here</h2>
          <textarea v-model="inputText"></textarea>
        </div>
        <div class="right">
          <h2>Show here</h2>
          <div class="text" v-html="outputText"></div>
        </div>
      </div>
      <div class="button-group">
        <button class="verify" @click="handleVerify">Verify</button>
        <button @click="handleCancel">Cancel</button>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, watch, getCurrentInstance, onMounted } from "vue";
import useDebouncedRef from "../composables/UserDebouncedRef";

import Parse from "../composables/LatexParse";
export default {
  name: "Edit",
  props: {
    defaultText: {
      type: String,
      default: "",
    },
  },
  emits: ["verify", "cancel"],
  setup(props, { emit }) {
    // latex 支持
    const { ctx: _this } = getCurrentInstance();
    const reRender = () => {
      _this.$forceUpdate();
      MathJax.Hub.Typeset();
    };
    onMounted(reRender);

    const inputText = useDebouncedRef(props.defaultText, 300);
    const outputText = ref(Parse(inputText.value));
    watch(inputText, () => {
      outputText.value = Parse(inputText.value);
      setTimeout(reRender, 10);
    });
    const handleVerify = () => {
      emit("verify", inputText.value);
    };
    const handleCancel = () => {
      emit("cancel");
    };

    return { inputText, outputText, handleVerify, handleCancel };
  },
};
</script>

<style scoped>
.edit-component {
  position: fixed;
  z-index: 10;
  top: 0;
  left: 0;
  width: 100vw;
  min-height: 100vh;
  background: rgba(0, 0, 0, 0.5);
  padding: 40px 0;
}
.edit-container {
  background: #252525;
  border-top: 3px solid #c49b3b;
  border-radius: 10px;
  padding: 40px;
  color: #aaa;
  font-size: 25px;
}
.edit-container h1 {
  text-align: center;
  color: #fff;
  font-size: 50px;
  margin-bottom: 20px;
}
.edit-container h2 {
  text-align: center;
  color: #fff;
  color: #fff;
  font-size: 25px;
  margin-bottom: 20px;
}
.edit-container .edit-stage {
  display: flex;
  flex-direction: row;
  height: 400px;
}
.edit-container .left {
  display: flex;
  flex-direction: column;
  z-index: 1;
  position: relative;
  width: 50%;
  max-height: 100%;
  padding: 0 10px;
  /* border: 3px solid crimson; */
}
.edit-container .left textarea {
  line-height: 1.5;
  background: transparent;
  box-shadow: none;
  resize: none;
  max-width: 100%;
  /* width: auto; */
  height: 100%;
  border: 1px solid #aaa;
  color: #aaa;
  font-size: 25px;
  padding: 0 10px;
}
.edit-container .left textarea:focus {
  background-color: #999;
  color: #fff;
  border: 1px solid cyan;
}
.edit-container .right {
  display: flex;
  flex-direction: column;
  z-index: 1;
  position: relative;
  width: 50%;
  max-height: 100%;
  padding: 0 10px;
  /* max-width: 100%; */
}
.edit-container .right .text {
  display: block;
  line-height: 1.5;
  background: transparent;
  overflow: scroll;
  /* overflow-x: auto; */
  /* overflow-x: hidden; */
  max-width: 100%;
  height: 100%;
  border: 1px solid #aaa;
  color: #aaa;
  font-size: 25px;
  padding: 0 10px;
  word-break: break-all;
}

.edit-container .button-group {
  margin-top: 40px;
  text-align: center;
}
.edit-container button {
  background: crimson;
  border: none;
  width: 70px;
  height: 30px;
  font-size: 15px;
  font-weight: 500;
  border-radius: 10px;
  cursor: pointer;
  color: #fff;
  transition: all 0.3s ease;
}
.edit-container button.verify {
  background: green;
  margin-right: 20px;
}
.edit-container button:hover {
  transform: scale(1.05);
}
</style>
