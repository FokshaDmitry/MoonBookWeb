﻿var cover = document.getElementById("Cover");
var efile = document.getElementById("EpubFile");
var coverImg = document.getElementById("CoverImg")
let tmpb;
// Add cover img
cover.addEventListener("change", () => {
    let f = cover.files[0];
    if (f) {
        coverImg.src = URL.createObjectURL(f);
        localStorage.setItem('myImage', coverImg.src);
    }
    coverImg.src = localStorage.getItem('myImage')
})
//add book of epub file
efile.addEventListener("change", () => {
    if (efile.files[0]) {
        const formData = new FormData();
        formData.append("efaile", efile.files[0]);
        fetch("/Books/eBook", {
            method: "PUT",
            body: formData
        }).then(r => r.json()).then(j => {
            if (j.status = "Ok") {
                //Show information book on epub file
                document.getElementById("Title").value = j.info.title;
                document.getElementById("Author").value = j.info.author;
                document.getElementById("ContentText").value = j.info.textContent;
                //Show cover
                if (j.cover) {
                    coverImg.src = `data:image/png;base64,` + j.cover;
                }
            }
        })
    }
})
