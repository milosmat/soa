package com.example.BlogApi.Model;

import lombok.*;
import org.springframework.data.annotation.Id;
import org.springframework.data.mongodb.core.mapping.Document;

import java.time.Instant;
import java.util.*;
import jakarta.validation.constraints.NotBlank;


@Data
@AllArgsConstructor
@Builder
@Document(collection = "blogs")
public class Blog {
    @Id
    private String id;
    @NotBlank(message = "Title is required")
    private String title;
    @NotBlank(message = "Description is required")
    private String description;
    private Instant createdAt;


    private List<String> images = new ArrayList<>();
    private List<Comment> comments = new ArrayList<>();
    private Set<String> likedBy = new HashSet<>();

    public Blog(){}

}
