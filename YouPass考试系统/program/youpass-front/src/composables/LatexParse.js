// latex语法翻译
const Parse = (s) => {
    s = s.replaceAll("\n", "<br>")
    const part = s.split("$");
    var ret = "<p>"
    for (var i = 0; i < part.length; i++) {
        if (i % 2 == 0) {
            ret += part[i];
        } else {
            ret +=
                "<sapn class='math inline'>\\(" + part[i] + "\\)</sapn>";
        }
    }
    ret += "</p>";
    return ret;
};

export default Parse;