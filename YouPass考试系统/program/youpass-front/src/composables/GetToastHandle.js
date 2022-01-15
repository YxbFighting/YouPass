import {ref} from 'vue';

const getToastHandle=()=>{
    // toast tag
    const showToast=ref(false);
    const isError=ref(true);
    const text=ref("")

    const triggerErrorToast = (t) => {
        showToast.value = true;
        isError.value=true;
        text.value=t;
        setTimeout(() => showToast.value = false, 3000)
    }
    const triggerGoodToast = (t) => {
        showToast.value = true;
        isError.value=false;
		text.value = t;
        setTimeout(() => showToast.value = false, 3000)
    }

    return {text,showToast,isError,triggerErrorToast,triggerGoodToast};
}

export default getToastHandle