<template>
  <div
    class="
      question-box
      col-xl-8
      offset-xl-2
      col-lg-8
      offset-lg-2
      col-sm-10
      offset-sm-1
      text-center
    "
  >
    <h2>{{ question.id }}</h2>
    <div v-html="question.content"></div>
  </div>
  <div class="choice-question" v-if="question.type === 0">
    <div
      class="
        answer-box
        col-xl-8
        offset-xl-2
        col-lg-8
        offset-lg-2
        col-sm-10
        offset-sm-1
        text-center
      "
      v-for="option in question.options"
      :key="option"
      @click="onClick($event, option.option_id)"
      :class="{ picked: picked.includes(option.option_id) }"
    >
      <div v-html="option.content"></div>
    </div>
  </div>
  <div class="choice-question" v-else-if="question.type === 1">
    <div
      class="
        answer-box
        col-xl-8
        offset-xl-2
        col-lg-8
        offset-lg-2
        col-sm-10
        offset-sm-1
        text-center
      "
      v-for="option in question.options"
      :key="option"
      @click="onClick($event, option.option_id)"
      :class="{ picked: pickedList.includes(option.option_id) }"
    >
      <div v-html="option.content"></div>
    </div>
  </div>

  <div class="input-question" v-else>
    <div
      class="buttonarea"
      v-if="isEditing_3 === 'beforeEditing' || isEditing_3 === 'afterEditing'"
    >
      <!-- <div class="addbutton">
        <button @click="toGetImg()">+</button>
      </div> -->
      <div class="editbutton">
        <button @click="isEditing_3 = 'isEditing'">Edit</button>
      </div>
      <!-- <div class="latexbutton">
        <button>Latex</button>
      </div> -->
    </div>

    <div
      class="
        field-textarea
        col-xl-8
        offset-xl-2
        col-lg-8
        offset-lg-2
        col-sm-10
        offset-sm-1
      "
    >
      <textarea
        id="text"
        cols="30"
        rows="10"
        placeholder="请在此输入答案..."
        v-model="inputText"
        required
        v-if="isEditing_3 === 'isEditing'"
      ></textarea>
      <div class="buttonarea">
        <div class="confirmbutton">
          <button @click="confirm_3" v-if="isEditing_3 === 'isEditing'">
            确认
          </button>
        </div>
      </div>
      <div>
        <div
          v-if="
            (isEditing_3 === 'afterEditing' ||
              isEditing_3 === 'beforeEditing') &&
            inputText.length
          "
        >
          <p>你的答案是：</p>
          <div v-html="outputText" class="output-text"></div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, getCurrentInstance, onMounted, watch } from "vue";
import Parse from "../../composables/LatexParse";
import useDebouncedRef from "../../composables/UserDebouncedRef";

export default {
  name: "Question",
  props: {
    question: Object,
  },
  emits: ["select", "unselect", "fillin", "addphoto"],
  setup(props, { emit }) {
    // 单选题的已选选项
    const picked = ref(props.question.picked);
    // 多选题的已选择题目集合
    const pickedList = ref(props.question.pickedList);
    // 填空题的已输入内容
    const written_blank = ref(props.question.written_blank);
    // 处理鼠标点击事件
    const onClick = (e, id) => {
      if (props.question.type === 0) {
        emit("select", id); //*******写emit传数据 */
      } else if (props.question.type === 1) {
        if (pickedList.value.includes(id) === true) {
          emit("unselect", id);
        } else {
          emit("select", id);
        }
      }
      picked.value = props.question.picked;
      pickedList.value = props.question.pickedList;
    };

    // 显示latex需要的函数
    // 问题题干和选项进行latex转换
    props.question.content = Parse(props.question.content);
    props.question.options.forEach((value, index) => {
      props.question.options[index].content = Parse(
        props.question.options[index].content
      );
    });
    const { ctx: _this } = getCurrentInstance();
    // 重新加载latex的函数
    const reRender = () => {
      _this.$forceUpdate();
      MathJax.Hub.Typeset();
    };
    // 在mount的时候 调用reRender
    onMounted(reRender);
    // 输入修改textarea
    const inputText = useDebouncedRef(props.question.written_blank, 1000);
    watch(inputText, () => {
      outputText.value = Parse(inputText.value);
      setTimeout(reRender, 10);
      console.log(outputText.value);
      emit("fillin", inputText.value);
    });
    const outputText = ref(Parse(inputText.value));
    const isEditing = ref("beforeEditing");
    const confirm = () => {
      isEditing.value = "afterEditing";
      setTimeout(reRender, 10);
    };

    const isEditing_3 = ref("beforeEditing");
    const confirm_3 = () => {
      isEditing_3.value = "afterEditing";
      setTimeout(reRender, 10);
    };
    // 上传图片
    // const driverUpload = () => {
    //   console.log("click");
    //   $("#input_upload_driver").click();
    // };
    let inputElement = null;
    let valueUrl = "";
    const toGetImg = () => {
      // 上传图片
      if (inputElement === null) {
        // 生成文件上传的控件
        inputElement = document.createElement("input");
        inputElement.setAttribute("type", "file");
        inputElement.style.display = "none";
        inputElement.addEventListener("change", uploadFile(), false);
        document.body.appendChild(inputElement);
      }
      inputElement.click();
    };
    const uploadFile = () => {
      console.log("123");
    };

    return {
      onClick,
      picked,
      pickedList,
      inputText,
      outputText,
      confirm,
      written_blank,
      isEditing,
      isEditing_3,
      confirm_3,
      toGetImg,
      valueUrl,
      uploadFile,
    };
  },
};
</script>

<style scoped>
.question-box {
  /* border: 3px solid #fff; */
  /* margin-top: 100px; */
  font-size: 20px;
  padding: 40px;
  background: #191919;
  border-radius: 10px;
  border-top: 3px solid #c49b3b;
  color: #aaa;
}
.question-box h2 {
  text-align: center;
  color: #c49b3b;
  font-size: 40px;
  font-weight: 1000;
  margin-bottom: 10px;
}
.answer-box {
  /* border: 3px solid #fff; */
  margin: 20px auto;
  font-size: 20px;
  padding: 10px 20px;
  background: #191919;
  border-radius: 10px;
  border-left: 20px solid #aaa;
  color: #aaa;
  cursor: pointer;
  word-break: break-all;
  transition: all 0.3s ease;
}
.answer-box.picked {
  border-left: 20px solid #00994c;
}
.answer-box:hover {
  transform: scale(1.01);
  border-left: 20px solid #00994c;
}
.input-question {
  margin: 20px auto;
  font-size: 20px;
  padding: 10px 20px;
  border-radius: 10px;
  transition: all 0.3s ease;
}
.input-question .field-textarea textarea {
  height: 250px;
  width: 100%;
  border: 3px solid black;
  border-radius: 10px;
  outline: none;
  resize: none;
  padding: 5px 15px;
  font-size: 17px;
  font-family: "Poppins", sans-serif;
  margin: 10px auto;
  background: #aaa;
  color: #000;
  font-weight: 600;
  font-size: 20px;
  transition: all 0.3s ease;
}
.input-question .field-textarea textarea:hover {
  background: #111;
  color: #fff;
  border: 3px solid white;
  transition: all 0.3s ease;
}
.input-question .buttonarea {
  text-align: center;
}
.input-question .output-text {
  background: #aaa;
  color: #000;
  font-weight: 600;
  font-size: 20px;
  font-family: "Poppins", sans-serif;
  border-radius: 10px;
  white-space: pre-line;
  word-break: break-word;
  padding: 5px 15px;
  border: 3px solid black;
}
/* .input-question .addbutton {
	margin-right: 20px ;
  margin-bottom: 15px;
  height: 40px;
  width: 45px;
  display: inline-block;
}
.input-question .addbutton button {
  height: 100%;
  width: 100%;
  border: 2px solid crimson;
  background: crimson;
  color: #111;
  font-size: 30px;
  font-weight: 500;
  border-radius: 6px;
  cursor: pointer;
  transition: all 0.3s ease;
}
.input-question .addbutton button:hover {
  color: crimson;
  background: none;
} */
.input-question .editbutton {
  margin: 20px auto;
  margin-bottom: 0px;
  height: 40px;
  width: 150px;
  display: inline-block;
}
.input-question .confirmbutton {
  margin-bottom: 15px;
  height: 50px;
  width: 150px;
  margin-left: auto;
}
.input-question .confirmbutton button {
  height: 100%;
  width: 100%;
  border: 2px solid crimson;
  background: crimson;
  color: white;
  font-size: 1.5em;
  font-weight: 500;
  border-radius: 10px;
  cursor: pointer;
  transition: all 0.3s ease;
  margin-top: 5px;
}
.input-question .confirmbutton button:hover {
  background: #ff033e;
  transform: scale(1.05);
  cursor: pointer;
}
.input-question .editbutton button {
  font-style: italic;
  height: 100%;
  width: 100%;
  border: 2px solid crimson;
  background: crimson;
  color: #111;
  font-size: 26px;
  font-weight: 500;
  border-radius: 6px;
  cursor: pointer;
  transition: all 0.3s ease;
}
.input-question .editbutton button:hover {
  color: crimson;
  background: none;
}
</style>
