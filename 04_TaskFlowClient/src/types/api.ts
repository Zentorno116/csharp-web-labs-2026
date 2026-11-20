export interface UserDto {
  id: number;
  username: string;
  email: string;
  role: "admin" | "user";
  createdAtUtc: string;
}

export interface AuthResponseDto {
  accessToken: string;
  expiresAtUtc: string;
  user: UserDto;
}

export interface ProjectDto {
  id: number;
  name: string;
  description: string;
  status: "Active" | "Completed" | "Archived";
  createdAtUtc: string;
  ownerId: number;
  ownerUsername: string;
  taskCount: number;
}

export interface TagDto {
  id: number;
  name: string;
  color: string;
}

export interface TaskItemDto {
  id: number;
  title: string;
  description: string;
  status: "Todo" | "InProgress" | "Done";
  priority: "Low" | "Medium" | "High";
  dueDateUtc: string | null;
  createdAtUtc: string;
  projectId: number;
  projectName: string;
  assigneeId: number | null;
  assigneeUsername: string | null;
  commentCount: number;
  tags: TagDto[];
}

export interface CommentDto {
  id: number;
  content: string;
  createdAtUtc: string;
  taskItemId: number;
  authorId: number;
  authorUsername: string;
}
