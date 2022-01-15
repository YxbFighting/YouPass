<template>
  <div class="toast-group">
    <transition name="error" appear>
      <Toast
        v-if="showToast && isError == true"
        :isError="isError"
        :text="text"
      />
    </transition>
    <transition name="good" appear>
      <Toast
        v-if="showToast && isError == false"
        :isError="isError"
        :text="text"
      />
    </transition>
  </div>
</template>

<script>
import Toast from "../components/Toast";

export default {
  name: "ToastGroup",
  props: {
    showToast: Boolean,
    isError: Boolean,
    text: String,
  },
  components: { Toast },
};
</script>

<style scoped>
/* .toast-group {
  position: fixed;
  width: 100vh;
  border: 3px solid crimson;
} */
</style>
