<template>
  <div class="backdrop">
    <div class="block">
      <div class="notice">
        <p>请输入密码:</p>
      </div>

      <input type="password" @keyup.enter="handleSubmit" v-model="message" />
      <div class="button-area">
        <div class="submit-button" @click="handleSubmit">确定</div>

        <div class="submit-button" @click="handleCancel">取消</div>
      </div>
    </div>
  </div>
</template>


<script>
import { ref } from "vue";
export default {
  name: "Block",

  emits: ["submit", "cancel"],

  setup(props, { emit }) {
    const message = ref("");
    const handleSubmit = () => {
      emit("submit", message.value);
      message.value=""
    };
    const handleCancel = () => {
      emit("cancel");
    };

    return {
      handleSubmit,
      handleCancel,
      message,
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
  padding-bottom: 20px;
  font-size: 18px;
}
.block input {
  border: 1px solid #555;
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