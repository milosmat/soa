package com.example.BlogApi.Model;

import lombok.*;
import org.springframework.data.annotation.Id;
import java.time.Instant;


@Data
@AllArgsConstructor
@NoArgsConstructor
@Builder
public class Comment {
    @Id
    private String id;
    private String authorId;
    private String authorName;
    private String text;
    private Instant createdAt;
    private Instant updatedAt; // vreme poslednje izmene
}
