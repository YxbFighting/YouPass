import axios from "axios";

const MyPost_File = async(path, formData) => {
    var data;
    await axios({
        method: "post",
        url: path,
        headers: { 'Content-Type': 'multipart/form-data' },
        data: formData,
        withCredentials: true,
    }).then((response) => {
        data = response.data;
    }).catch(function(response) {
        //handle error
        console.log(response);
    });;
    if (data == undefined) {
        return { "status": "Error" }
    } else { return data; }
}

export default MyPost_File