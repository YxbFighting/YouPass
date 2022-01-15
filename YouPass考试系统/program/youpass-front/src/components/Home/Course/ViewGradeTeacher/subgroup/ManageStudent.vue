<template>
  <div class="manages">
    <div
      class="
        display-port
        col-xl-6
        offset-xl-3
        col-lg-8
        offset-lg-2
        col-sm-10
        offset-sm-1
        text-center
      "
    >
      <span class="back-icon material-icons" @click="triggerBack">
        arrow_back
      </span>
      <div class="content">
        <h2>{{ courseTitle }}</h2>
        <p>双击可删除学生</p>
        <div class="body">
          <div class="body-head">
            <div class="left">学生编号</div>
            <div class="right">学生姓名</div>
          </div>
          <div class="body-table">
            <table>
              <!-- <thead>
                         <th>学生编号</th>
                         <th>学生姓名</th>
                     </thead> -->
              <tbody>
                <tr
                  v-for="student in studentList"
                  :key="student"
                  @dblclick="handledbclick(student)"
                  class="stuinfo"
                >
                  <td>{{ student.studentid }}</td>
                  <td>{{ student.studentname }}</td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { onMounted, ref } from "vue";
import MyGet from "../../../../../composables/MyGet";
import BackendPath from "../../../../../composables/BackendPath";
import MyDelete from "../../../../../composables/MyDelete";
export default {
  props: {
    courseId: String,
    courseTitle: String,
  },
  emits: ["triggerBack"],
  setup(props, { emit }) {
    const triggerBack = () => {
      emit("triggerBack");
    };
    const studentList = ref([]);
    onMounted(async () => {
      console.log(
        BackendPath + "api/Course/StuGet?course_id=" + props.courseId
      );
      const data = await MyGet(
        BackendPath + "api/Course/StuGet?course_id=" + props.courseId
      );
      if (data.status === "Good") {
        data.student_list.forEach((value, index) => {
          studentList.value.push({
            studentid: value.stu_id,
            studentname: value.stu_name,
          });
        });
      }
    });

    const handledbclick = async (student) => {
      console.log(student.studentid);
      const data = await MyDelete(
        BackendPath +
          "api/Course/DeleteStu?student_id=" +
          student.studentid +
          "&course_id=" +
          props.courseId
      );
      console.log(data);
      if (data.status === "Good") {
        studentList.value = studentList.values.filter((value) => {
          return value.studentid != student.studentid;
        });
        // studentList.value=studentList.value.findIndex(item=>item===student)<0?studentList:studentList.value.splice(studentList.value.findIndex(item=>item===student),1)
      }
    };
    return {
      triggerBack,
      studentList,
      handledbclick,
    };
  },
};
</script>

<style scoped>
.manages .back-icon {
  margin: auto 30px auto 0;
  font-size: 25px;
  cursor: pointer;
  margin-right: 30px;
  /* transition: all 0.3s ease; */
}

.manages .back-icon:hover {
  color: crimson;
  /* transform: scale(1.2); */
}
.manages {
  position: fixed;
  z-index: 999;
  top: 0;
  left: 0;
  color: #aaa;
  background: rgba(0, 0, 0, 0.5);
  width: 100vw;
  height: 100vh;
  padding: 30px 0;
}
.manages .display-port {
  background: #252525;
  height: 100%;
  border-radius: 20px;
  padding: 50px;
}
.display-port .content {
  text-align: center;
}
.display-port .content p {
  font-size: 25px;
  margin-top: 10px;
}
.display-port .content h2 {
  text-align: center;
  font-size: 40px;
  color: #fff;
  margin-bottom: 20px;
}
.body {
  height: 550px;
}

.body table {
  border-collapse: separate;
  border-spacing: 10px;
  margin: 0 auto;
  width: 100%;
  text-align: center;
}
.body table th {
  font-size: 30px;
}
.body table tr {
  font-size: 25px;
}
.body-head {
  display: flex;
  font-size: 30px;
}
.body-head .left {
  width: 50%;
  display: inline-block;
  text-align: center;
}
.body-head .right {
  width: 50%;
  display: inline-block;
  text-align: center;
}
.body-table {
  padding: 0;
  height: 100%;
  overflow-y: scroll;
}
.body-table::-webkit-scrollbar {
  width: 0.05px;
}
.stuinfo:hover {
  cursor: pointer;
  color: crimson;
  transition: 0.5s;
}
</style>