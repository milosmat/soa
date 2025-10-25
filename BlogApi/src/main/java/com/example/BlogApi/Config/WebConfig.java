package com.example.BlogApi.Config;

import org.springframework.beans.factory.annotation.Value;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.web.servlet.config.annotation.CorsRegistry;
import org.springframework.web.servlet.config.annotation.ResourceHandlerRegistry;
import org.springframework.web.servlet.config.annotation.WebMvcConfigurer;

import java.util.Arrays;

@Configuration
public class WebConfig {

    @Value("${app.cors.allowed-origins:http://localhost:4200}")
    private String allowedOrigins;

    @Value("${app.upload-dir:./uploads}")
    private String uploadDir;

    @Bean
    public WebMvcConfigurer corsConfigurer() {
        return new WebMvcConfigurer() {
            @Override
            public void addCorsMappings(CorsRegistry registry) {
                String[] origins = Arrays.stream(allowedOrigins.split(","))
                        .map(String::trim)
                        .filter(s -> !s.isEmpty())
                        .toArray(String[]::new);
                registry.addMapping("/**")
                        .allowedOrigins(origins)
            .allowedMethods("GET", "POST", "PUT", "DELETE", "PATCH", "OPTIONS")
            .allowedHeaders("*")
            .exposedHeaders("Location")
            .maxAge(3600)
                        .allowCredentials(true);
            }

            @Override
            public void addResourceHandlers(ResourceHandlerRegistry registry) {
                // Serve uploaded files under /uploads/**
                String location = "file:" + (uploadDir.endsWith("/") ? uploadDir : uploadDir + "/");
                registry.addResourceHandler("/uploads/**")
                        .addResourceLocations(location)
                        .setCachePeriod(3600);
            }
        };
    }
}
