<template>
  <div class="select-component" @click.self="handleCancel">
    <div
      class="select-container col-xxl-8
        offset-xxl-2
        col-xl-8
        offset-xl-2
        col-lg-8
        offset-lg-2
        col-sm-10
        offset-sm-1"
    >
      <h1>Select</h1>
      <div class="select-stage">
        <div v-if="inputType === 0">
          <div
            class="answer-box"
            :class="{ selected: selectId === item.id }"
            v-for="item in inputOption"
            :key="item"
            v-html="item.contentHtml"
            @click="onSelect(item.id)"
          ></div>
        </div>
        <div v-else>
          <div
            class="answer-box"
            :class="{ selected: selectIdList.includes(item.id) }"
            v-for="item in inputOption"
            :key="item"
            v-html="item.contentHtml"
            @click="onSelect(item.id)"
          ></div>
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
import { ref, getCurrentInstance, onMounted } from "vue";
export default {
  name: "Answer",
  props: {
    inputOption: {},
    inputType: Number,
  },
  emits: ["cancel", "verify"],
  setup(props, { emit }) {
    // latex 支持
    const { ctx: _this } = getCurrentInstance();
    const reRender = () => {
      _this.$forceUpdate();
      MathJax.Hub.Typeset();
    };
    onMounted(reRender);

    const selectId = ref(null);
    const selectIdList = ref([]);
    const onSelect = (id) => {
      if (props.inputType == 0) {
        selectId.value = id;
      } else {
        if (selectIdList.value.includes(id)) {
          selectIdList.value = selectIdList.value.filter(
            (value) => value != id
          );
        } else {
          selectIdList.value.push(id);
        }
      }
    };

    const handleVerify = () => {
      if (props.inputType === 0) {
        emit("verify", selectId.value);
      } else {
        emit("verify", selectIdList.value);
      }
    };
    const handleCancel = () => {
      emit("cancel");
    };
    return { handleVerify, handleCancel, onSelect, selectId, selectIdList };
  },
};
</script>

<style scoped>
.select-component {
  position: fixed;
  z-index: 10;
  top: 0;
  left: 0;
  width: 100vw;
  min-height: 100vh;
  background: rgba(0, 0, 0, 0.5);
  padding: 40px 0;
}
.select-container {
  background: #252525;
  border-top: 3px solid #c49b3b;
  border-radius: 10px;
  padding: 40px;
  color: #aaa;
  font-size: 25px;
}
.select-container h1 {
  text-align: center;
  color: #fff;
  font-size: 50px;
  margin-bottom: 20px;
}
.select-container .select-stage {
  display: block;
  height: 400px;
  overflow: scroll;
  overflow-x: hidden;
}
.select-container .select-stage .answer-box {
  padding: 10px 0;
  border-radius: 10px;
  margin-bottom: 10px;
  cursor: pointer;
  transition: all 0.3s ease;
}
.select-container .select-stage .answer-box:hover {
  background: crimson;
}
.select-container .select-stage .answer-box.selected {
  background: crimson;
}
.select-container button.verify {
  background: green;
  margin-right: 20px;
}
.select-container .button-group {
  margin-top: 40px;
  text-align: center;
}
.select-container button {
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
.select-container button:hover {
  transform: scale(1.05);
}
</style>
