<template>
  <ExamManagement
    v-if="onShow"
    :triggerErrorToast="triggerErrorToast"
    :triggerGoodToast="triggerGoodToast"
    :courseId="courseId"
    :examId="examId"
    :identity="identity"
    @triggerBack="handleTrigger"
  />
  <div class="exam-list" v-else>
    <div class="header">
      <span class="back-icon material-icons" @click="triggerBack">
        arrow_back
      </span>
      <div class="text">{{ courseId }}</div>
      /
      <div class="text">考试列表</div>
    </div>
    <div class="body-container">
      <div class="body">
        <div class="exam-card-stage">
          <div
            class="card"
            v-for="exam in examList"
            :key="exam"
            @click="onClick(exam.exam_id)"
          >
            <div class="title">{{ exam.title }}</div>
            <div class="exam-id">{{ exam.exam_id }}</div>
            <div class="time"><span>Start</span> {{ exam.start_time }}</div>
            <div class="time"><span>End</span> {{ exam.start_time }}</div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { onMounted, ref } from "vue";
import { useRouter, useRoute } from "vue-router";

import MyGet from "@/composables/MyGet";
import BackendPath from "@/composables/BackendPath";
import ExamManagement from "./ExamManagement/ExamManagement.vue";

export default {
  components: { ExamManagement },
  props: {
    triggerErrorToast: { type: Function },
    triggerGoodToast: { type: Function },
    courseId: Number,
    identity: Number,
  },
  emits: ["triggerBack"],
  setup(props, { emit }) {
    const triggerBack = () => {
      emit("triggerBack");
    };

    const examDetail = ref([
      { title: "Correct Papers", handle: "CorrectPapers" },
    ]);
    // const chooseDetail = (handle) => {
    //   onShow.value = handle;
    // };
    const handleTrigger = () => {
      onShow.value = false;
      examId.value = null;
    };

    var examList = ref([]);

    const triggerErrorToast = props.triggerErrorToast;
    const triggerGoodToast = props.triggerGoodToast;

    onMounted(async () => {
      // TODO:
      // api要更换
      const data = await MyGet(
        BackendPath + "api/exam/GetbyCourseId?course_id=" + props.courseId
      );
      if (data.status === "Good") {
        data.info_Exam.forEach((value, index) => {
          examList.value.push({
            course_id: value.course_id,
            exam_id: value.exam_id,
            title: value.title,
            start_time: value.start_time,
            end_time: value.end_time,
          });
        });
      } else {
        triggerErrorToast("get error");
      }
    });

    const onShow = ref(false);
    const examId = ref(null);
    const onClick = (exam_id) => {
      examId.value = parseInt(exam_id);
      onShow.value = true;
    };

    return {
      triggerBack,
      examDetail,
      onShow,
      // chooseDetail,
      handleTrigger,
      examList,
      onClick,
      examId,
    };
  },
};
</script>

<style scoped src="../../../../../public/css/Course.css"></style>
<style scoped>
.body {
  padding: 40px;
}
.exam-card-stage .card {
  display: inline-block;
  width: 300px;
  height: 300px;
  margin: 10px 10px;
  background: #191919;
  border-radius: 20px;
  border-top: 2px solid #c49b3b;
  cursor: pointer;
  transition: all 0.3s ease;
}
.exam-card-stage .card:hover {
  transform: scale(1.05);
}
.exam-card-stage .card .title {
  display: block;
  margin: 50px auto;
  font-size: 30px;
  color: #c49b3b;
  text-align: center;
  overflow: hidden;
}
.exam-card-stage .card .exam-id {
  display: block;
  margin: 20px auto;
  text-align: center;
  font-size: 20px;
  color: #fff;
}
.exam-card-stage .card .time {
  display: block;
  margin: 20px auto;
  text-align: left;
  margin-left: 20px;
}
.exam-card-stage .card .time span {
  display: inline-block;
  font-size: 20px;
  color: #fff;
  margin-right: 20px;
  width: 50px;
  overflow: hidden;
  /* border: 3px solid crimson; */
}
</style>
