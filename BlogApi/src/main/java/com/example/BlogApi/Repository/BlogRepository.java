package com.example.BlogApi.Repository;

import com.example.BlogApi.Model.Blog;
import org.springframework.data.mongodb.repository.MongoRepository;
import org.springframework.stereotype.Repository;


@Repository
public interface BlogRepository extends MongoRepository<Blog, String>{

}
