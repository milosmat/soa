export interface Comment {
  id: string;
  authorId: string;
  authorName?: string;
  text: string;
  createdAt: string; 
  updatedAt: string; 
}

export interface Blog {
  id: string;
  title: string;
  description: string; 
  createdAt: string; 
  images: string[];
  comments: Comment[];
  likedBy: string[];
}

export interface CreateBlogReq {
  title: string;
  description: string;
  images?: string[];
}

export interface AddCommentReq {
  text: string;
}
