package com.example.BlogApi.Controller;


import com.example.BlogApi.Model.Blog;
import com.example.BlogApi.Model.Comment;
import com.example.BlogApi.Service.BlogService;
import lombok.RequiredArgsConstructor;
import org.springframework.security.core.annotation.AuthenticationPrincipal;
import io.micrometer.core.annotation.Timed;
import org.springframework.security.oauth2.jwt.Jwt;
import org.springframework.http.ResponseEntity;
import org.springframework.validation.annotation.Validated;
import org.springframework.web.bind.annotation.*;

import jakarta.validation.Valid;
import java.net.URI;
import java.util.List;

@RestController
@RequestMapping("/api/blogs")
@RequiredArgsConstructor
@Validated
public class BlogController {
    private final BlogService blogService;

    @PostMapping
    @Timed(value = "blogs.create", description = "Time to create a blog")
    public ResponseEntity<Blog> createBlog(@Valid @RequestBody Blog blog) {
        Blog created = blogService.createBlog(blog);
        return ResponseEntity.created(URI.create("/api/blogs/" + created.getId())).body(created);
    }

    @GetMapping
    @Timed(value = "blogs.list", description = "List blogs request")
    public ResponseEntity<List<Blog>> listBlogs() {
        return ResponseEntity.ok(blogService.listBlogs());
    }

    @GetMapping("/{id}")
    @Timed(value = "blogs.get", description = "Get single blog")
    public ResponseEntity<Blog> getBlog(@PathVariable String id) {
        return blogService.getBlog(id).map(ResponseEntity::ok).orElseGet(() -> ResponseEntity.notFound().build());
    }

    @DeleteMapping("/{id}")
    @Timed(value = "blogs.delete", description = "Delete blog")
    public ResponseEntity<Void> deleteBlog(@PathVariable String id) {
        blogService.deleteBlog(id);
        return ResponseEntity.noContent().build();
    }

    @PostMapping("/{id}/comments")
    @Timed(value = "blogs.comment.add", description = "Add comment")
    public ResponseEntity<Comment> addComment(@PathVariable String id,
                                              @RequestBody Comment comment,
                                              @AuthenticationPrincipal Jwt jwt) {
        if (jwt == null || jwt.getSubject() == null) return ResponseEntity.status(401).build();
        comment.setAuthorId(jwt.getSubject());
        Object uname = jwt.getClaims().getOrDefault("unique_name", null);
        if (uname == null) uname = jwt.getClaims().getOrDefault("preferred_username", null);
        if (uname == null) uname = jwt.getClaims().getOrDefault("name", null);
        if (uname != null) comment.setAuthorName(String.valueOf(uname));
        Comment added = blogService.addComment(id, comment);
        return ResponseEntity.ok(added);
    }

    @PutMapping("/{id}/comments/{commentId}")
    @Timed(value = "blogs.comment.update", description = "Update comment")
    public ResponseEntity<Comment> updateComment(@PathVariable String id, @PathVariable String commentId, @RequestBody String newText) {
        return blogService.updateComment(id, commentId, newText)
                .map(ResponseEntity::ok)
                .orElseGet(() -> ResponseEntity.notFound().build());
    }

    @PostMapping("/{id}/like")
    @Timed(value = "blogs.like", description = "Like blog")
    public ResponseEntity<Integer> like(@PathVariable String id, @AuthenticationPrincipal Jwt jwt) {
        if (jwt == null || jwt.getSubject() == null) return ResponseEntity.status(401).build();
        String userId = jwt.getSubject();
        int count = blogService.like(id, userId);
        return ResponseEntity.ok(count);
    }

    @PostMapping("/{id}/unlike")
    @Timed(value = "blogs.unlike", description = "Unlike blog")
    public ResponseEntity<Integer> unlike(@PathVariable String id, @AuthenticationPrincipal Jwt jwt) {
        if (jwt == null || jwt.getSubject() == null) return ResponseEntity.status(401).build();
        String userId = jwt.getSubject();
        int count = blogService.unlike(id, userId);
        return ResponseEntity.ok(count);
    }
}