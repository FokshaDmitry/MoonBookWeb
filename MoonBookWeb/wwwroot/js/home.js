﻿document.addEventListener("DOMContentLoaded", () => {
	//Element <post><post/>
	let post = document.querySelector("post");
	if (!post) throw "Forum  script: APP not found";
	let posts = new Posts("/api/freand/Post");
	//load All Post
	posts.loadElement(post);
});