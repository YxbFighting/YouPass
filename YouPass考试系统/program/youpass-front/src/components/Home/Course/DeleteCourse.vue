<template>
  <div class="backdrop">
    <div class="block">
      <div class="notice">
        <p>确定要删除课程吗？</p>
      </div>
      <div class="button-area">
        <div class="submit-button" @click="handleSubmit">确定</div>
        <div class="submit-button" @click="handleCancel">取消</div>
      </div>
    </div>
  </div>
</template>

<script>
import { ref } from "vue";
import MyDelete from "../../../composables/MyDelete";
import BackendPath from "../../../composables/BackendPath";

export default {
  name: "DeleteCourse",
  emits: ["submit","cancel"],
  props: {
    triggerErrorToast: { type: Function },
    triggerGoodToast: { type: Function },
    courseId: String,
  },
  setup(props, { emit }) {
    const message = ref("");

    const triggerErrorToast = props.triggerErrorToast;
    const triggerGoodToast = props.triggerGoodToast;

    // const handleSubmit = async () => {
    //   const data = await MyDelete(BackendPath + "api/Course", {
    //     course_id: props.courseId,
    //   });
    const handleSubmit = async () => {
     const data = await MyDelete(BackendPath + "api/Course?"+"course_id="+props.courseId)

      emit('submit')
      if (data.status === "Good") {
        triggerGoodToast("删除成功");
      } else if (data.status === "Bad") {
        triggerErrorToast("删除失败");
      } else triggerErrorToast("");
    };
    const handleCancel = () => {
      emit("cancel");
    };
    return {
      handleSubmit,
      handleCancel,
    };
  },
};
</script>

<style scoped>
.block {
  width: 400px;
  padding: 20px;
  margin: 300px;
  background: white;
  border-radius: 10px;
  color: #333;
}
.block .notice {
  text-align: center;
  padding: 20px 0;
  font-size: 18px;
}

.block .button-area {
  text-align: center;
}
.block .button-area .submit-button {
  display: inline-block;
  margin: 0 20px;
  width: 80px;
  height: 40px;
  border: 2px solid #dc143c;
  border-radius: 10px;
  background: #dc143c;
  text-align: center;
  line-height: 40px;
  color: #fff;
  font-size: 16px;
  cursor: pointer;
}
.block .button-area .submit-button:hover {
  background: #fff;
  color: #dc143c;
  transition: all 0.2s ease;
}
.backdrop {
  top: 0;
  position: fixed;
  background: rgba(0, 0, 0, 0.5);
  width: 100%;
  height: 100%;
}
</style>