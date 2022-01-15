<template>
  <CorrectPapers
    v-if="onShow === 1"
    :triggerErrorToast="triggerErrorToast"
    :triggerGoodToast="triggerGoodToast"
    :courseId="courseId"
    :examId="examId"
    @triggerBack="handleTrigger"
  />
  <div class="exam-management" v-else>
    <div class="header">
      <span class="back-icon material-icons" @click="triggerBack">
        arrow_back
      </span>
      <div class="text">{{ courseId }}</div>
      /
      <div class="text">考试列表</div>
      /
      <div class="text">考试管理({{ examId }})</div>
    </div>
    <div class="body-container">
      <div class="body">
        <div class="exam-function-list">
          <div v-if="identity === 0">
            <div
              class="function"
              v-for="item in teacherFunctionList"
              :key="item.id"
              @click="onClick(item.id)"
            >
              {{ item.title }}
            </div>
          </div>
          <div v-else>
            <div
              class="function"
              v-for="item in studentFunctionList"
              :key="item.id"
              @click="onClick(item.id)"
            >
              {{ item.title }}
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { ref } from "vue";

import CorrectPapers from "./CorrectPapers/CorrectPapers.vue";

export default {
  components: { CorrectPapers },
  props: {
    triggerErrorToast: { type: Function },
    triggerGoodToast: { type: Function },
    courseId: Number,
    examId: Number,
    identity: Number,
  },
  emits: ["triggerBack"],
  setup(props, { emit }) {
    const triggerBack = () => {
      emit("triggerBack");
    };
    const handleTrigger = () => {
      onShow.value = null;
    };

    // 写死功能
    const teacherFunctionList = ref([{ title: "批改试卷", id: 1 }]);
    const studentFunctionList = ref([]);

    const onShow = ref(null);
    const onClick = (id) => {
      onShow.value = id;
    };

    return {
      triggerBack,
      teacherFunctionList,
      studentFunctionList,
      onClick,
      onShow,
      handleTrigger,
    };
  },
};
</script>

<style scoped src="@/../public/css/Course.css"></style>

<style scoped>
.body {
  padding: 40px;
}
.body .function {
  display: flex;
  background: #191919;
  border-top: 2px solid #c49b3b;
  min-height: 100px;
  border-radius: 20px;
  font-size: 50px;
  padding: 20px;
  margin-bottom: 20px;
  cursor: pointer;
}
.body .function:hover {
  /* transform: scale(1.01); */
  /* background: #fff; */
  background: #555;
}
.body .function .text {
  margin: auto;
}
</style>
