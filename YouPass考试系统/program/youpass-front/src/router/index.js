import { createRouter, createWebHistory } from 'vue-router'
import Index from '../views/Index.vue'
import Signup from "../views/Signup.vue"
import Login from "../views/Login.vue"
import Test from "../views/Test.vue"
import Home from "../views/Home.vue"
import Exam from "../views/Exam.vue"
import NotFound from "../views/NotFound.vue"

const routes = [{
        path: '/',
        name: 'Index',
        component: Index
    },
    {
        path: '/signup/:type?',
        name: 'Signup',
        component: Signup,
        props: (route) => ({ type: Number.parseInt(route.params.type) || 0 })
    },
    {
        path: '/login',
        name: 'Login',
        component: Login
    },
    {
        path: '/test',
        name: 'Test',
        component: Test
    },
    {
        // path: '/home/:id',
        path: '/home',
        name: 'Home',
        component: Home,
        // props: (route) => ({ id: route.params.id })
    },
    {
        path: '/exam',
        name: 'Exam',
        component: Exam,
    },
    // catchall 404
    {
        path: "/:catchAll(.*)",
        name: "NotFound",
        component: NotFound
    }
]

const router = createRouter({
    history: createWebHistory(process.env.BASE_URL),
    routes
})

export default router