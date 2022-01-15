<template>
  <div class="take-exam">
    <h1>我的考试</h1>
    <div class="exam-card-stage">
      <div
        class="card"
        v-for="exam in examList"
        :key="exam"
        @click="onClick(exam)"
      >
        <div class="title">{{ exam.title }}</div>
        <div class="course-id">{{ exam.course_id }}</div>
        <div class="time"><span>Start</span> {{ exam.start_time }}</div>
        <div class="time"><span>End</span> {{ exam.end_time }}</div>
      </div>
    </div>
  </div>
</template>

<script>
import { onMounted, ref } from "vue";
import { useRouter, useRoute } from "vue-router";

import MyGet from "@/composables/MyGet";
import MyPost from "@/composables/MyPost.js";
import BackendPath from "@/composables/BackendPath";
export default {
  props: {
    triggerErrorToast: { type: Function },
    triggerGoodToast: { type: Function },
  },
  setup(props, { emit }) {
    // use route and router
    const router = useRouter();
    const route = useRoute();

    var examList = ref([]);

    const triggerErrorToast = props.triggerErrorToast;
    const triggerGoodToast = props.triggerGoodToast;

    onMounted(async () => {
      const data = await MyGet(BackendPath + "api/exam");
      console.log(1, data);
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

    const onClick = async (exam) => {
      const data = await MyPost(BackendPath + "api/Takeexam", {
        exam_id: exam.exam_id,
        course_id: exam.course_id,
        //  "title":exam.title,
        //  "starttime":exam.start_time,
        //  "endtime":exam.end_time
      });
      if (data.status != "Good") {
        props.triggerErrorToast("Error");
      } else {
        router.push({ name: "Exam" });
      }

      // console.log(data)
      // router.push({ name: "Exam",params:{examid:exam.exam_id,courseid:exam.course_id,title:exam.title,starttime:exam.start_time,endtime:exam.end_time}})
    };

    return { examList, triggerGoodToast, triggerErrorToast, onClick };
  },
};
</script>

<style scoped>
.take-exam {
  width: 100%;
  padding: 40px;
}
.take-exam h1 {
  color: #fff;
  padding-bottom: 20px;
  border-bottom: 3px solid #bbb;
}
.take-exam .exam-card-stage {
  margin-top: 20px;
  /* border: 3px solid crimson; */
}
.take-exam .exam-card-stage .card {
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
.take-exam .exam-card-stage .card:hover {
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
.exam-card-stage .card .course-id {
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
