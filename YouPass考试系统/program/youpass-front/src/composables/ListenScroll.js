import debounce from "lodash/debounce";
import { onBeforeUnmount } from "vue";

const ListenScroll = (handleScroll) => {
    const handleDebouncedScroll = debounce(handleScroll, 10);
    window.addEventListener("scroll", handleDebouncedScroll);
    onBeforeUnmount(() => {
        window.removeEventListener("scroll", handleDebouncedScroll);
    });
}

export default ListenScroll