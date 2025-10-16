export type Role = 'GUIDE' | 'TOURIST' | 'ADMIN';

export interface AuthResponse {
  token: string;
}

export interface RegisterReq {
  username: string;
  email: string;
  password: string;
  role: Role;
}

export interface LoginReq {
  usernameOrEmail: string;
  password: string;
}
