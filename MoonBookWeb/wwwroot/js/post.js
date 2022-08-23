﻿var Title = document.getElementById("Title");
var Text = document.getElementById("Text");
var TextComment = document.getElementById("TextComment");
var Post = document.getElementById("Post");
var ImagePost = document.getElementById("ImagePost");
var valTitle = "";

document.addEventListener("DOMContentLoaded", () => {
	const post = document.querySelector("post");  
	if (!post) throw "Forum  script: APP not found";
	var posts = new Posts("/api/post");
	posts.loadElement(post);
	
});

Title.addEventListener("click", () => {
		var start = Text.selectionStart;
		var end = Text.selectionEnd;
		valTitle = Text.value.substring(start, end);
		Text.value = Text.value.substr(0, start) + ">" + Text.value.substr(start, end) + "<" + Text.value.substr(end);
		Text.setSelectionRange(end, end);
})
Post.addEventListener("click", () => {
	const formData = new FormData();
	formData.append("TitlePost", valTitle);
	formData.append("TextPost", Text.value);
	formData.append("ImagePost", ImagePost.files[0]);
	fetch("/API/Post", {
		method: "POST",
		body: formData
	}).then(r => r.json()).then(j => {
		if (j.status == "Error") {
			alert(j.message)
		} else {
			location.reload();
		}
	})
});