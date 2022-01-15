<template>
  <!-- 子组件（删除） -->
  <DeleteCourse
    v-if="showDelete"
    :triggerErrorToast="triggerErrorToast"
    :triggerGoodToast="triggerGoodToast"
    :courseId="courseId"
    @submit="submitDelete"
    @cancel="cancelDelete"
  />
  <CourseInfo
    v-if="onShow === 'CourseInfo'"
    :triggerErrorToast="triggerErrorToast"
    :triggerGoodToast="triggerGoodToast"
    :courseId="courseId"
    :courseTitle="courseTitle"
    @triggerBack="handleTrigger"
  />
  <ViewGrade
    v-else-if="onShow === 'ViewGrade'"
    :triggerErrorToast="triggerErrorToast"
    :triggerGoodToast="triggerGoodToast"
    :courseId="courseId"
    :courseTitle="courseTitle"
    @triggerBack="handleTrigger"
  />
  <AddQuestions
    v-else-if="onShow === 'AddQuestion'"
    :triggerErrorToast="triggerErrorToast"
    :triggerGoodToast="triggerGoodToast"
    :courseId="courseId"
    @triggerBack="handleTrigger"
  />
  <ExamList
    v-else-if="onShow === 'ExamList'"
    :triggerErrorToast="triggerErrorToast"
    :triggerGoodToast="triggerGoodToast"
    :courseId="courseId"
    :identity="identity"
    @triggerBack="handleTrigger"
  />
  <ReleaseTest
    v-else-if="onShow == 'ReleaseTest'"
    :triggerErrorToast="triggerErrorToast"
    :triggerGoodToast="triggerGoodToast"
    :courseId="courseId"
    :courseTitle="courseTitle"
    @triggerBack="handleTrigger"
  />
  <ViewGradeT
    v-else-if="onShow == 'ViewGradeT'"
    :triggerErrorToast="triggerErrorToast"
    :triggerGoodToast="triggerGoodToast"
    :courseId="courseId"
    :courseTitle="courseTitle"
    @triggerBack="handleTrigger"
  />
  <!-- <ViewGrade 

  /> -->
  <div class="details" v-else>
    <div class="header">
      <span class="back-icon material-icons" @click="triggerBack">
        arrow_back
      </span>
      <div class="text">{{ courseId }}</div>
      <!-- else infomation -->
    </div>
    <div class="body-container">
      <div class="body">
        <!-- if student 当获得全部数据的时候 需要加v-if -->
        <div class="student" v-if="identity === 1">
          <div
            class="function"
            v-for="item in detailStudent"
            :key="item"
            @click="chooseDetail(item.handle)"
          >
            <div class="text">
              {{ item.title }}
            </div>
          </div>
        </div>
        <!-- if teacher -->
        <div class="teacher" v-else>
          <div
            class="function"
            v-for="item in detailTeacher"
            :key="item"
            @click="chooseDetail(item.handle)"
          >
            <div class="text">
              {{ item.title }}
            </div>
          </div>

          <!-- Delete Course 选项 -->
          <div class="teacher">
            <div class="function-2" @click="handleDelete">
              <div class="text">删除课程</div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { ref } from "vue";
import ReleaseTest from "./ReleaseTest.vue";
import AddQuestions from "./AddQuestions/AddQuestions.vue";
import CourseInfo from "./CourseInfo.vue";
import ExamList from "./ExamList/ExamList.vue";
import MyDelete from "../../../composables/MyDelete";
import BackendPath from "../../../composables/BackendPath";
import ViewGrade from "./ViewGrade.vue";
import DeleteCourse from "./DeleteCourse.vue";
import ViewGradeT from "./ViewGradeTeacher/ViewGradeT.vue";

export default {
  name: "Details",
  components: {
    AddQuestions,
    ExamList,
    ReleaseTest,
    DeleteCourse,
    ViewGrade,
    CourseInfo,
    ViewGradeT,
  },

  emits: ["triggerBack"],
  props: {
    triggerErrorToast: { type: Function },
    triggerGoodToast: { type: Function },
    courseId: Number,
    courseTitle: String,
    identity: Number,
  },
  setup(props, { emit }) {
    const triggerBack = () => {
      emit("triggerBack");
    };
    const onShow = ref(null);

    const detailStudent = ref([
      { title: "查看成绩", handle: "ViewGrade" },
      { title: "查看考试", handle: "ExamList" },
    ]);

    const detailTeacher = ref([
      { title: "课程管理", handle: "CourseInfo" },
      { title: "发布考试", handle: "ReleaseTest" },
      { title: "上传题目", handle: "AddQuestion" },
      { title: "查看考试", handle: "ExamList" },
      { title: "查看学生成绩", handle: "ViewGradeT" },
    ]);

    const chooseDetail = (handle) => {
      onShow.value = handle;
    };
    //check
    const handleTrigger = () => {
      onShow.value = null;
    };

    //删除课程
    const showDelete = ref(false);
    const handleDelete = () => {
      showDelete.value = true;
    };
    const submitDelete = () => {
      showDelete.value = false;
      triggerBack();
    };
    const cancelDelete = () =>{
      showDelete.value = false;
    }

    return {
      triggerBack,
      onShow,
      handleTrigger,
      detailStudent,
      detailTeacher,
      chooseDetail,
      handleDelete,
      showDelete,
      submitDelete,
      cancelDelete
    };
  },
};
</script>

<style scoped src="../../../../public/css/Course.css"></style>

<style scoped>
.body {
  padding: 40px;
}
.body .function {
  display: flex;
  background: #191919;
  border-top: 2px solid #c49b3b;
  min-height: 70px;
  border-radius: 15px;
  font-size: 36px;
  padding: 20px;
  cursor: pointer;
  width: 70%;
  margin: auto;
  margin-bottom: 30px;
}
.body .function:hover {
  /* transform: scale(1.01); */
  /* background: #fff; */
  background: #555;
  color: #fff;
  transition: all 0.3s ease;
}
.body .function .text {
  margin: auto;
}

/* 删除 */
.body .function-2 {
  background: #f13a3a;
  display: flex;
  min-height: 70px;
  border-radius: 15px;
  font-size: 36px;
  padding: 20px;
  cursor: pointer;
  width: 70%;
  margin: auto;
  margin-bottom: 30px;
  color: #fff;
}
.body .function-2 .text {
  margin: auto;
}
.body .function-2:hover {
  background: #ff6a6a;
  color: #fff;
  transition: all 0.3s ease;
}
</style>
