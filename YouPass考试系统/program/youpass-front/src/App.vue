<template>
  <router-view />
</template>

<script>
export default {};
</script>

<style>
@import url("https://fonts.googleapis.com/css2?family=Ma+Shan+Zheng&family=Poppins:ital,wght@1,300;1,500&family=Ubuntu:ital,wght@1,300;1,500;1,700&family=ZCOOL+KuaiLe&display=swap");
/* font-family: 'Ma Shan Zheng', cursive;
font-family: 'Poppins', sans-serif;
font-family: 'Ubuntu', sans-serif;
font-family: 'ZCOOL KuaiLe', cursive; */
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
  text-decoration: none;
}
html {
  scroll-behavior: smooth;
  transition: all 0.3 ease;
}
.max-width {
  max-width: 1300px;
  padding: 0 80;
  margin: auto;
}

@keyframes wobble {
  0% {
    transform: translateY(-100px);
    opacity: 0;
  }
  50% {
    transform: translateY(0px);
    opacity: 1;
  }
  60% {
    transform: translateX(8px);
    opacity: 1;
  }
  70% {
    transform: translateX(-8px);
    opacity: 1;
  }
  80% {
    transform: translateX(4px);
    opacity: 1;
  }
  90% {
    transform: translateX(-4px);
    opacity: 1;
  }
  100% {
    transform: translateX(0px);
    opacity: 1;
  }
}

.error-leave-to {
  opacity: 0;
  transform: translateY(-60px);
}
.error-leave-active {
  transition: all 0.3s ease;
}
.error-enter-active {
  animation: wobble 0.5s ease;
}

.good-enter-from,
.good-leave-to {
  opacity: 0;
  transform: translateY(-60px);
}
.good-enter-active,
.good-leave-active {
  transition: all 0.3s ease;
}
</style>
