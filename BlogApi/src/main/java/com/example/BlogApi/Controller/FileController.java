package com.example.BlogApi.Controller;

import org.springframework.beans.factory.annotation.Value;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.util.StringUtils;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.multipart.MultipartFile;

import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.nio.file.StandardCopyOption;
import java.time.Instant;
import java.util.HashMap;
import java.util.Map;
import java.util.UUID;
import org.springframework.web.servlet.support.ServletUriComponentsBuilder;

@RestController
@RequestMapping("/api/blogs")
public class FileController {

    @Value("${app.upload-dir:./uploads}")
    private String uploadDir;

    @PostMapping("/upload")
    public ResponseEntity<Map<String, String>> upload(@RequestParam("file") MultipartFile file) {
        if (file.isEmpty()) return ResponseEntity.badRequest().build();
        try {
            Path root = Paths.get(uploadDir);
            Files.createDirectories(root);
            String original = StringUtils.cleanPath(file.getOriginalFilename() == null ? "file" : file.getOriginalFilename());
            String ext = "";
            int dot = original.lastIndexOf('.');
            if (dot >= 0) ext = original.substring(dot);
            String name = UUID.randomUUID() + "-" + Instant.now().toEpochMilli() + ext;
            Path target = root.resolve(name);
            Files.copy(file.getInputStream(), target, StandardCopyOption.REPLACE_EXISTING);
        String absoluteUrl = ServletUriComponentsBuilder.fromCurrentContextPath()
            .path("/uploads/")
            .path(name)
            .toUriString();
        Map<String,String> body = new HashMap<>();
        body.put("url", absoluteUrl);
            return ResponseEntity.status(HttpStatus.CREATED).body(body);
        } catch (IOException e) {
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).build();
        }
    }
}
