<template>
  <div class="create">
    <h1>创建课程</h1>
    <div class="create-card">
      <div class="title">
        <div class="label-1">课程名称：</div>
        <div class="input-1"><input type="text" v-model="title" /></div>
        <div class="error">{{ titleError }}</div>
      </div>

      <div class="title">
        <div class="label-1">课程密码：</div>
        <div class="input-1"><input type="password" v-model="password1" /></div>
        <div v-if="passwordError1" class="error">{{ passwordError1 }}</div>
      </div>
      <div class="title">
        <div class="label-1">确认密码：</div>
        <div class="input-1"><input type="password" v-model="password2" /></div>
        <div v-if="passwordError2" class="error">{{ passwordError2 }}</div>
      </div>
      <div class="submit">
        <div class="submit-button" @click="handleSubmit">创建</div>
        <div class="submit-button" @click="handleClear">重置</div>
      </div>
    </div>
  </div>
</template>

<script>
import { ref } from "vue";
import BackendPath from "@/composables/BackendPath";
import MyPost from "../../../composables/MyPost";

export default {
  props: {
    triggerErrorToast: { type: Function },
    triggerGoodToast: { type: Function },
  },
  setup(props) {
    const triggerErrorToast = props.triggerErrorToast;
    const triggerGoodToast = props.triggerGoodToast;

    const title = ref("");
    const titleError = ref("");

    const password1 = ref("");
    const passwordError1 = ref("");
    const password2 = ref("");
    const passwordError2 = ref("");
    const course_id = ref("");

    const handleSubmit = async () => {
      let errorNum = 0;
      if (title.value.length === 0) {
        titleError.value = "Title can't be empty";
        errorNum++;
      } else titleError.value = "";
      if (password1.value.length === 0) {
        passwordError1.value = "Password can't be empty";
        errorNum++;
      } else passwordError1.value = "";
      if (password1.value != password2.value) {
        passwordError2.value = "Two passwords are not the same";
        errorNum++;
      } else passwordError2.value = "";

      if (errorNum === 0) {
        const data = await MyPost(BackendPath + "api/Course", {
          title: title.value,
          password: password1.value,
        });
        if (data.status === "Good")
          triggerGoodToast("创建成功 " + data.generated_Course_Id);
        else if (data.status === "Bad") triggerErrorToast("创建失败");
        else triggerErrorToast("");
        handleClear();
      }
    };
    const handleClear = () => {
      title.value = "";
      password1.value = "";
      password2.value = "";
      titleError.value=""
      passwordError1.value=""
      passwordError2.value=""
    };
    return {
      title,
      course_id,
      password1,
      password2,
      titleError,
      passwordError1,
      passwordError2,
      handleSubmit,
      handleClear,
    };
  },
};
</script>

<style>
.create {
  width: 100%;
  padding: 40px;
}
.create h1 {
  color: white;
  padding-bottom: 20px;
  border-bottom: 3px solid #bbb;
}
.create .create-card {
  width: 900px;
  height: 600px;
  margin: 30px auto;
  background: #191919;
  border-radius: 20px;
  border-top: 2px solid #c49b3b;
  transition: all 0.3s ease;
  color: #ddd;
  text-align: center;
}
.create .create-card .title {
  margin-top: 70px;
  font-size: 24px;
}
.create .create-card .label-1 {
  display: inline-block;
}
.create .create-card .input-1 {
  display: inline-block;
  width: 400px;
  margin-left: 50px;
}
.create .create-card .input-1 input {
  background: #eee;
}
.create .create-card .error {
  color: crimson;
  font-size: 20px;
}
.create .create-card .submit {
  margin-top: 70px;
  text-align: center;
}
.create .create-card .submit .submit-button {
  display: inline-block;
  width: 100px;
  height: 50px;
  margin: 0 40px;
  background: #dc143c;
  border: 2px solid #dc143c;
  border-radius: 10px;
  line-height: 50px;
  font-size: 19px;
}
.create .create-card .submit .submit-button:hover {
  cursor: pointer;
  background: #212121;
  color: #dc143c;
  transition: all 0.3s ease;
}
</style>
