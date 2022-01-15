<template>
  <div class="todos">
    <input
      type="text"
      v-model="newTodo"
      @keypress.enter="addTodo"
      placeholder="Add a new todo..."
    />
    <transition name="switch" mode="out-in">
      <div v-if="todos.length">
        <transition-group tag="ul" name="list" appear>
          <li v-for="todo in todos" :key="todo.id" @click="deleteTodo(todo.id)">
            {{ todo.text }}
          </li>
        </transition-group>
      </div>
      <div v-else>Woohoo, nothing left todo!</div>
    </transition>
  </div>
</template>

<script>
import { ref, onMounted } from "vue";

import BackendPath from "../../../composables/BackendPath";
import myGet from "../../../composables/MyGet";
import MyPost from "../../../composables/MyPost";
import MyPut from "../../../composables/MyPut";
import MyDelete from "../../../composables/MyDelete";

export default {
  setup(props, { emit }) {
    const todos = ref([]);
    const newTodo = ref("");

    onMounted(async () => {
      const data = await myGet(BackendPath + "api/todolist");
      // console.log(data)
      data.todolist.forEach((value, index) => {
        todos.value.push({ text: value.description, id: value.id });
      });
      todos.value.sort();
      todos.value.reverse();
    });

    const addTodo = async () => {
      if (newTodo.value) {
        const data = await MyPut(BackendPath + "api/todolist", {
          description: newTodo.value,
        });
        if (data.status === "Good") {
          const id = data.new_id;
          todos.value = [{ text: newTodo.value, id }, ...todos.value];
          newTodo.value = "";
        } else {
          emit("badPost");
        }
      } else {
        emit("badValue");
      }
    };

    const deleteTodo = async (id) => {
      const data = await MyDelete(BackendPath + "api/todolist?" + "id=" + id);
      // console.log(BackendPath + "api/todolist?" + "id=" + id);
      // console.log(id, data);
      if (data.status === "Good") {
        todos.value = todos.value.filter((todo) => todo.id != id);
      } else {
        emit("badDelete");
      }
    };

    return { todos, addTodo, deleteTodo, newTodo };
  },
};
</script>

<style>
.todos {
  max-width: 400px;
  margin: 20px auto;
  position: relative;
}
input {
  width: 100%;
  padding: 12px;
  border: 1px solid #eee;
  border-radius: 10px;
  box-sizing: border-box;
  margin-bottom: 20px;
}
input:focus {
  outline: none;
}
.todos ul {
  position: relative;
  padding: 0;
  /* border: 3px solid black; */
}
.todos li {
  list-style-type: none;
  display: block;
  margin-bottom: 10px;
  padding: 10px;
  background: #fff;
  color: #1b1b1b;
  box-shadow: 1px 3px 5px rgba(0, 0, 0, 0.1);
  border-radius: 10px;
  width: 100%;
  box-sizing: border-box;
  /* border: 1px dashed crimson; */
}
.todos li:hover {
  cursor: pointer;
}

/* list transitions */
.list-enter-from {
  opacity: 0;
  transform: scale(0.6);
}
.list-enter-to {
  opacity: 1;
  transform: scale(1);
}
.list-enter-active {
  transition: all 0.4s ease;
}
.list-leave-from {
  opacity: 1;
  transform: scale(1);
}
.list-leave-to {
  opacity: 0;
  transform: scale(0.6);
}
.list-leave-active {
  transition: all 0.4s ease;
  position: absolute;
}

.list-move {
  transition: all 0.3s ease;
}

/* switch transition */
.switch-enter-from,
.switch-leave-to {
  opacity: 0;
  transform: translateY(20px);
}
/* .switch-enter-to,
.switch-leave-from {
  opacity: 1;
  transform: translateY(0);
} */
.switch-enter-active,
.switch-leave-active {
  transition: all 0.5s ease;
}
</style>
