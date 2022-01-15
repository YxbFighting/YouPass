import debounce from "lodash/debounce";
import { onBeforeUnmount } from "vue";

const ListenResize = (handleResize) => {
    const handleDebouncedResize = debounce(handleResize, 10);
    window.addEventListener("resize", handleDebouncedResize);
    onBeforeUnmount(() => {
        window.removeEventListener("resize", handleDebouncedResize);
    });
}

export default ListenResize