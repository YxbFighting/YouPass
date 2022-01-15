<template>
  <div class="CreateExam">
      this is create exam
  </div>
</template>

<script>
import { onMounted, ref } from "vue";

import BackendPath from "../../composables/BackendPath";
export default {
     props: {
    id: String,
    triggerErrorToast: { type: Function },
    triggerGoodToast: { type: Function },
  },
}
</script>

<style>

</style>