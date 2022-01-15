<template>
  <div class="scoretable">
    <div class="score-place">
      <p>学生得分 :</p>
      <input
        class="num"
        type="number"
        min="0"
        :max="maxScore"
        required
        v-model="score"
      />
    </div>
    <div class="submit">
      <button @click="onclick">下一位学生</button>
    </div>
  </div>
</template>

<script>
import { ref, onUpdated, watch } from "vue";
export default {
  props: {
    maxScore: Number,
    preScore: Number,
  },
  emits: ["next", "scoreChanged"],
  setup(props, { emit }) {
    const score = ref(props.preScore); //得分
    watch(score, () => {
      if (score.value < 0) {
        score.value = 0;
      } else if (score.value > props.maxScore) {
        score.value = props.maxScore;
      } else if (typeof score.value === "string") {
        if (score.value === "") {
          score.value = 0;
        } else {
          score.value = parseInt(score.value);
        }
      }
      emit("scoreChanged", score.value);
    });
    watch(
      () => props.preScore,
      () => {
        score.value = props.preScore;
      }
    );
    //按钮的事件
    const onclick = () => {
      emit("next");
    };

    return {
      score,
      onclick,
    };
  },
};
</script>

<style scoped>
/* 整体的样式 */
.scoretable {
  /* position: relative; */
  display: flex;
  flex-direction: column;
  justify-content: center;
  background: #191919;
  height: 100%;
  border-radius: 10px;
  padding: 0 30px;
  /* padding-top: 100px; */
  /* padding-bottom: 210px; */
  border: 3px solid black;
  align-items: center;
  text-align: center;
}
/* 分数赋予区的css */
.scoretable .score-place {
  display: flex;
}
/* 分数赋予区的p */
.scoretable .score-place p {
  width: 50%;
  color: white;
  font-size: 25px;
}
/* 分数赋予区的num */
.scoretable .score-place .num {
  width: 50%;
  font-size: 25px;
  font-weight: bold;
  border: none;
}
/* 提交区 */
.scoretable .submit {
  padding: 50px;
}
/* 按钮样式 */
.scoretable .submit button {
  border: none;
  background: #aaa;
  padding: 10px;
  font-size: 20px;
  border-radius: 10px;
  cursor: pointer;
  transition: all 0.3s ease;
}
.scoretable .submit button:hover {
  background: crimson;
}
</style>
