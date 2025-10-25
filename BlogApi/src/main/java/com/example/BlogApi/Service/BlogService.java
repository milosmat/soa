package com.example.BlogApi.Service;


import com.example.BlogApi.Model.Blog;
import com.example.BlogApi.Model.Comment;
import com.example.BlogApi.Repository.BlogRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.data.mongodb.core.MongoTemplate;
import org.springframework.data.mongodb.core.query.Criteria;
import org.springframework.data.mongodb.core.query.Query;
import org.springframework.data.mongodb.core.query.Update;
import org.springframework.stereotype.Service;

import java.time.Instant;
import java.util.*;

@Service
@RequiredArgsConstructor
public class BlogService {
    private final BlogRepository blogRepository;
    private final MongoTemplate mongoTemplate;

    public Blog createBlog(Blog blog){
        blog.setId(null);
        blog.setCreatedAt(Instant.now());
        if (blog.getComments() == null)
            blog.setComments(new ArrayList<>());
        if (blog.getImages() == null)
            blog.setImages(new ArrayList<>());
        return blogRepository.save(blog);
    }

    public Optional<Blog> getBlog(String id) {
        return blogRepository.findById(id);
    }

    public List<Blog> listBlogs() {
        return blogRepository.findAll();
    }

    public void deleteBlog(String id) {
        blogRepository.deleteById(id);
    }

    public Comment addComment(String blogId, Comment comment) {
        comment.setId(UUID.randomUUID().toString());
        comment.setCreatedAt(Instant.now());
        comment.setUpdatedAt(Instant.now());

        Query q = Query.query(Criteria.where("_id").is(blogId));
        Update u = new Update().push("comments", comment);
        mongoTemplate.updateFirst(q, u, "blogs");
        return comment;
    }

    public Optional<Comment> updateComment(String blogId, String commentId, String newText) {
        Optional<Blog> ob = blogRepository.findById(blogId);
        if (ob.isEmpty()) return Optional.empty();
        Blog b = ob.get();
        for (Comment c : b.getComments()) {
            if (c.getId().equals(commentId)) {
                c.setText(newText);
                c.setUpdatedAt(Instant.now());
                blogRepository.save(b);
                return Optional.of(c);
            }
        }
        return Optional.empty();
    }

    public int like(String blogId, String userId) {
        Query q = Query.query(Criteria.where("_id").is(blogId).and("likedBy").ne(userId));
        Update u = new Update().addToSet("likedBy", userId);
        var res = mongoTemplate.updateFirst(q, u, Blog.class);
        Blog b = blogRepository.findById(blogId).orElse(null);
        return b == null ? 0 : b.getLikedBy().size();
    }

    public int unlike(String blogId, String userId) {
        Query q = Query.query(Criteria.where("_id").is(blogId));
        Update u = new Update().pull("likedBy", userId);
        mongoTemplate.updateFirst(q, u, Blog.class);
        Blog b = blogRepository.findById(blogId).orElse(null);
        return b == null ? 0 : b.getLikedBy().size();
    }

}
