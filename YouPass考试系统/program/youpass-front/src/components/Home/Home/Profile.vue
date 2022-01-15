<template>
  <div class="user-information" v-if="isLoaded === true">
    <h1>主页</h1>
    <div class="content">
      <div class="left col-md-5">
        <div class="card-body">
          <img
            class="profile-user-img"
            ref="imgRef"
            :src="userInformation.img"
            v-if="isEditing === false"
          />
          <img
            class="profile-user-img"
            ref="imgRef2"
            id="img"
            :key="newUserInfomation.img"
            :src="newUserInfomation.img"
            v-else
          />
          <transition name="fade-fixed" mode="out-in">
            <div>
              <input
                style="display: none"
                type="file"
                @change="onFileSelected"
                ref="fileInput"
              />
              <button
                v-if="isEditing"
                class="edit-image"
                @click="handleEditImage"
              >
                edit image
              </button>
            </div>
          </transition>
          <transition name="fade-fixed" mode="out-in"
            ><div v-if="isEditing === false" class="profile-name">
              {{ userInformation.name }}
            </div>
            <div v-else>
              <input
                class="edit-name"
                type="text"
                v-model="newUserInfomation.name"
              />
            </div>
          </transition>

          <div class="profile-infomation">
            <h2>About me</h2>
            <div class="profile-infomation-content">
              <strong><span class="material-icons"> email </span>Email</strong>
              <transition name="fade-fixed" mode="out-in"
                ><p v-if="!isEditing">{{ userInformation.email }}</p>
                <input
                  class="edit-other"
                  type="text"
                  v-else
                  v-model="newUserInfomation.email"
              /></transition>

              <div class="br"></div>
             
              <strong
                ><span class="material-icons"> room </span>Location</strong
              >
              <transition name="fade-fixed" mode="out-in">
                <p v-if="!isEditing">{{ userInformation.location }}</p>
                <input
                  class="edit-other"
                  type="text"
                  v-else
                  v-model="newUserInfomation.location"
                />
              </transition>

              <div class="br"></div>
            </div>
          </div>
          <transition name="fade-up" mode="out-in">
            <button v-if="!isEditing" @click="handleModify">Modify</button>
            <div v-else class="button-group">
              <button @click="handleVerify" class="verify">Verify</button>
              <button @click="handleCancel">Cancel</button>
            </div>
          </transition>
        </div>
      </div>
      <div class="right col-md-7">
        <div class="todo-stage">
          <h2>Todos</h2>
          <Todos
            @badValue="triggerErrorToast('can not input empty')"
            @badPost="triggerErrorToast('post error')"
            @badDelete="triggerErrorToast('delete error')"
          />
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, onMounted, getCurrentInstance } from "vue";

import Todos from "./Todos";
import BackendPath from "@/composables/BackendPath";
import myGet from "@/composables/MyGet";
import MyPost_File from "@/composables/MyPost_File";
import MyPut from "@/composables/MyPut";

export default {
  name: "Profile",
  props: {
    triggerErrorToast: { type: Function },
    triggerGoodToast: { type: Function },
  },
  components: {
    Todos,
  },
  setup(props, { refs }) {
    const triggerErrorToast = props.triggerErrorToast;
    const triggerGoodToast = props.triggerGoodToast;

    const userInformation = ref({
      img: "",
      name: "",
      email: "",
      education: "",
      location: "",
    });
    const newUserInfomation = ref();

    const isLoaded = ref(false);
    const imgRef = ref(null);
    const imgRef2 = ref(null);
    onMounted(async () => {
      const data = await myGet(BackendPath + "api/userinfomation");
      userInformation.value = {
        img: BackendPath + "api/userimage",
        name: data.name,
        email: data.email,
        location: data.location,
      };
      isLoaded.value = true;

      setTimeout(() => {
        imgRef.value.src =
          imgRef.value.src.split("?")[0] + "?" + new Date().getTime();
      }, 5);
    });

    // 表示是否正在修改
    const isEditing = ref(false);

    const handleModify = () => {
      isEditing.value = true;
      newUserInfomation.value = Object.assign({}, userInformation.value);
      // setTimeout(() => {
      //   imgRef2.value.src =
      //     imgRef2.value.src.split("?")[0] + "?" + new Date().getTime();
      // }, 10);
    };
    const handleCancel = () => {
      isEditing.value = false;
    };
    // 用户确认之后
    const handleVerify = async () => {
      try {
        // 发送图片
        if (imageFile.value != null) {
          const formData = new FormData();
          formData.append("files", imageFile.value);
          var imageData = await MyPost_File(
            BackendPath + "api/userimage",
            formData
          );
        }
        // 错误处理
        if (imageData != undefined && imageData.status == "Good") {
          userInformation.value.img = newUserInfomation.value.img;
        } else if (imageData != undefined && imageData.status != "Good") {
          triggerErrorToast("put image error!");
        }

        if (userInformation.value != newUserInfomation.value) {
          // 发送文本信息
          var textData = await MyPut(BackendPath + "api/userinfomation", {
            name: newUserInfomation.value.name,
            email: newUserInfomation.value.email,
            location: newUserInfomation.value.location,
          });
        }
        // 错误处理
        if (textData != undefined && textData.status == "Good") {
          const imgBefore = userInformation.value.img;
          userInformation.value = Object.assign({}, newUserInfomation.value);
          userInformation.value.img = imgBefore;
        } else if (textData != undefined && textData.status != "Good") {
          triggerErrorToast("put text error!");
        }
        // 结束修改
        isEditing.value = false;
        // 刷新
        // location.reload();
      } catch (err) {
        triggerErrorToast(err);
        //结束修改
        isEditing.value = false;
      }
    };

    // 为上传input添加ref，并重写点击事件
    const fileInput = ref(null);
    const handleEditImage = () => {
      fileInput.value.click();
    };

    // 上传图片
    const imageFile = ref(null);
    const onFileSelected = (e) => {
      const file = e.target.files[0];
      newUserInfomation.value.img = URL.createObjectURL(file);
      imageFile.value = file;
    };

    return {
      imgRef,
      imgRef2,
      isLoaded,
      userInformation,
      newUserInfomation,
      triggerGoodToast,
      triggerErrorToast,
      isEditing,
      handleModify,
      handleCancel,
      handleVerify,
      onFileSelected,
      handleEditImage,
      fileInput,
    };
  },
};
</script>

<style scoped>
.user-information {
  width: 100%;
  padding: 40px;
}
.user-information h1 {
  color: #fff;
}
.user-information .content {
  display: flex;
  width: 100%;
  margin-top: 30px;
}
.user-information .content .left {
  text-align: center !important;
}
.user-information .content .left .card-body {
  margin-right: 10px;
  background-color: #1b1b1b;
  border-top: 3px solid #c49b3b;
  border-radius: 10px;
  padding: 1.25rem;
}
.user-information .card-body .profile-user-img {
  display: block;
  border: 3px solid #adb5bd;
  margin: 0 auto;
  padding: 3px;
  width: 200px;
  height: 200px;
  border-radius: 50%;
  object-fit: cover;
}
.user-information .card-body .profile-name {
  margin-top: 10px;
  font-size: 40px;
}
.user-information .card-body .profile-infomation {
  display: block;
  text-align: left;
  margin-top: 50px;
}
.profile-infomation h2 {
  padding: 10px;
  color: #eee;
  border-bottom: 2px solid #aaa;
}
.profile-infomation .profile-infomation-content {
  display: block;
  padding: 20px 40px;
}
.profile-infomation .profile-infomation-content strong {
  font-size: 20px;
  font-weight: 400;
}
.profile-infomation .profile-infomation-content .material-icons {
  font-size: 20px;
  margin-right: 10px;
}
.profile-infomation .profile-infomation-content p {
  margin-top: 10px;
  font-size: 15px;
  font-weight: 300;
  padding-left: 10px;
}
.profile-infomation .profile-infomation-content .br {
  margin: 20px 0;
  display: block;
  height: 1px;
  background: #aaa;
}

/* right styling */
.user-information .content .right {
  text-align: center !important;
}
.user-information .content .right .todo-stage {
  background-color: #1b1b1b;
  border-top: 3px solid #c49b3b;
  border-radius: 10px;
  padding: 1.25rem;
}
.user-information .content .right h2 {
  font-size: 24px;
  color: #eee;
}
.user-information .card-body button {
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
.user-information .card-body button.verify {
  background: green;
  margin-right: 20px;
}
.user-information .card-body button.edit-image {
  background: #aaa;
  width: 100px;
  margin: 10px auto;
}
.user-information .card-body button:hover {
  transform: scale(1.05);
}

/* fade transition */
.fade-up-leave-to,
.fade-up-enter-from {
  opacity: 0;
  transform: translateY(-20px);
}
.fade-up-enter-active,
.fade-up-leave-active {
  transition: all 0.5s ease;
}

.fade-fixed-leave-to,
.fade-fixed-enter-from {
  opacity: 0;
}
.fade-fixed-enter-active,
.fade-fixed-leave-active {
  transition: all 0.5s ease;
}

/* input styling */
.user-information input {
  background: #aaa;
  border: none;
  margin-top: 10px;
}
.user-information input.edit-name {
  font-size: 25px;
  text-align: center;
}
.user-information input.edit-other {
  font-size: 15px;
  text-align: left;
  white-space: pre-wrap;
}
</style>
