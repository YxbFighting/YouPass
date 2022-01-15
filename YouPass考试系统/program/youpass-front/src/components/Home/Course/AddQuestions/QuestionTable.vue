<template>
  <div class="question-table">
    <div class="number-stage">
      <div class="number-group">
        <div
          class="question-number"
          :class="{ done: question.done }"
          v-for="question in questions.filter(
            (value) =>
              value.id >= maxQuestionInAView * onShowGroup + 1 &&
              value.id <= maxQuestionInAView * (onShowGroup + 1)
          )"
          :key="question.id"
          @click="onClick(question.id)"
        >
          {{ question.id }}
        </div>
      </div>
    </div>

    <div class="move-group" v-if="questions.length > maxQuestionInAView">
      <span class="icon left material-icons" @click="goLeft">
        chevron_left
      </span>
      <span class="icon right material-icons" @click="goRight">
        chevron_right
      </span>
    </div>

    <div class="button-group">
      <div class="plus-subtraction">
        <button class="plus" @click="plus">+</button>
        <button class="subtraction" @click="subtraction">-</button>
      </div>
      <div class="submit">
        <button @click="submit">submit</button>
      </div>
    </div>
  </div>
</template>

<script>
import { ref } from "vue";
export default {
  props: {
    questions: Array,
    maxQuestionInAView: Number,
    onShowGroup: Number,
  },
  emits: [
    "chooseQuestion",
    "plus",
    "subtraction",
    "goLeft",
    "goRight",
    "submit",
  ],
  setup(props, { emit }) {
    const onClick = (id) => {
      emit("chooseQuestion", id);
    };
    const plus = () => {
      emit("plus");
    };
    const subtraction = () => {
      emit("subtraction");
    };
    const goLeft = () => {
      emit("goLeft");
    };
    const goRight = () => {
      emit("goRight");
    };
    const submit = () => {
      emit("submit");
    };

    return { onClick, plus, subtraction, goLeft, goRight, submit };
  },
};
</script>

<style scoped>
.question-table {
  position: relative;
  background: #191919;
  height: 600px;
  border-radius: 10px;
  padding: 40px;
  padding-bottom: 210px;
}
.question-table .number-stage {
  max-height: 100%;
  /* border: 3px solid #fff; */
  overflow: hidden;
  /* border: 1px solid #aaa; */
  /* background: #c49b3b; */
}
.question-table .question-number {
  display: inline-block;
  background: #aaa;
  font-size: 10px;
  /* padding: auto auto; */
  width: 30px;
  height: 30px;
  line-height: 30px;
  border-radius: 50%;
  text-align: center;
  color: #000;
  margin: 10px;
  cursor: pointer;
  transition: all 0.3s ease;
}
.question-table .question-number.done:hover,
.question-table .question-number:hover {
  background: #fff;
}
.question-table .question-number.done {
  background: #00994c;
}
.question-table .move-group {
  height: 60px;
  width: 100%;
  left: 0;
  position: absolute;
  bottom: 150px;
  padding-top: 10px;
  /* border: 3px solid crimson; */
  display: flex;
  justify-content: center;
}
.question-table .move-group .icon {
  display: inline-block;
  font-size: 40px;
  cursor: pointer;
}
.question-table .move-group .icon.left {
  margin-right: 20px;
}
.question-table .move-group .icon:hover {
  color: crimson;
}
.question-table .button-group {
  height: 150px;
  width: 100%;
  left: 0;
  position: absolute;
  bottom: 0;
  /* border: 3px solid crimson; */
}
.question-table .button-group button {
  border: none;
  background: #aaa;
  padding: 10px;
  font-size: 20px;
  border-radius: 10px;
  cursor: pointer;
  transition: all 0.3s ease;
}
.question-table .button-group button:hover {
  background: crimson;
}
.question-table .button-group .plus-subtraction {
  display: flex;
  justify-content: center;
  margin: 10px 0 20px 0;
}
.question-table .button-group button.plus,
.question-table .button-group button.subtraction {
  display: inline-block;
  min-width: 40px;
}
.question-table .button-group button.plus {
  margin-right: 10px;
}
.question-table .button-group .submit {
  display: block;
  text-align: center;
  /* background: #c49b3b; */
}
</style>
