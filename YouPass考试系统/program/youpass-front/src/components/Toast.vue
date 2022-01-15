<template>
  <div class="toast-wrapper">
    <div class="toast" :class="{ good: !isError }">
      <div v-if="text">{{ text }}</div>
      <div v-else>something error...</div>
    </div>
  </div>
</template>

<script>
export default {
  name: "Toast",
  props: {
    isError: { type: Boolean, default: true },
    text: String,
  },
  setup(props) {},
};
</script>

<style>
.toast-wrapper {
  box-sizing: border-box;
  position: fixed;
  width: 100vw;
  left: 0px;
  top: 20px;
  z-index: 1000;
  font-size: 30px;
  /* border: 3px solid greenyellow; */
}
.toast {
  padding: 20px;
  color: white;
  background: #ff0062;
  border-radius: 10px;
  box-shadow: 1px 3px 5px rgba(0, 0, 0, 0.2);
  max-width: 400px;
  min-width: 400px;
  margin: 0 auto;
}
.toast.good {
  background: green;
}
</style>
