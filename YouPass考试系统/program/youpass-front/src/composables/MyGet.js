import axios from "axios";
// 执行正确返回正确结果，否则返回{status:"Error"}
// 如果是后端返回的错误，将会返回{status:"Bad"}
const MyGet = async (path) => {
    var data;
    await axios(path, {
        method: "get",
        withCredentials: true,
    }).then((response) => {
        data = response.data;
    }).catch(function (response) {
        //handle error
        console.log(response);
    });;
    if (data == undefined) {
        return { "status": "Error" }
    } else { return data; }
}

export default MyGet