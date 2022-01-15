<template>
  <div v-html="myHtml"></div>
</template>

<script>
import { ref, getCurrentInstance } from "vue";
import debounce from "lodash/debounce";
import { onBeforeUnmount } from "vue";
import axios from "axios";

import MyGet from "../composables/MyGet";
import MyPost from "../composables/MyPost";
import MyPost_File from "../composables/MyPost_File";
import MyPut from "../composables/MyPut";
import MyDelete from "../composables/MyDelete";
import BackendPath from "../composables/BackendPath";
import Parse from "../composables/LatexParse";

import CorrectQuestion from "../components/Home/Course/ExamList/ExamManagement/CorrectPapers/CorrectQuestion/CorrectQuestion.vue";

export default {
  components: { CorrectQuestion },
  setup() {
    // latex 支持
    const { ctx: _this } = getCurrentInstance();
    const reRender = () => {
      _this.$forceUpdate();
      MathJax.Hub.Typeset();
    };
    const myHtml = ref(
      Parse(
        "$\\begin{array}{cc}\\mathrm{Bad} & \\mathrm{Better} \\\\\\hline \\\\e^{i\\frac{\\pi}2} \\quad e^{\\frac{i\\pi}2}& e^{i\\pi/2} \\\\\\int_{-\\frac\\pi2}^\\frac\\pi2 \\sin x\\,dx & \\int_{-\\pi/2}^{\\pi/2}\\sin x\\,dx \\\\\\end{array}$"
      )
    );
    setTimeout(reRender, 100);
    return { myHtml };
  },
};
</script>

<style scoped>
.text-container {
  width: 100px;
  height: 100px;
  border: 3px solid crimson;
  word-break: break-all;
}
.test {
  width: 100%;
  background: crimson;
}
.show {
  background: #aaa;
}
</style>
