import { gql } from "@apollo/client";

export const GET_USER_INFO = gql`
query GetUser {
    user {
      username
      firstName
      lastName
      email
    }
  }
`;

export const USER_LOGIN = gql`
query Login($username: String!, $password: String!){
    token: login(login: {username: $username, password: $password})
}
`;

export const GET_CHATS = gql`
query GetChats($offset: Int!){
    userChats(skip: $offset) {
      items {
        chatId
        type
        name
        participants {
          items {
            username
            firstName
            lastName
          }
        }
        lastMessage {
          sender{
            username
            firstName
            lastName
          }
          time
          message
        }
      }
      pageInfo {
        hasNextPage
      }
    }
  }
`;

export const GET_CHAT_BY_ID = gql`
query GetChatById($chatId: String!, $chatType: ChatType){
    userChatByChatId(chatId: $chatId, chatType: $chatType, take: 1) {
      items {
        chatId
        type
        name
        participants {
          items {
            username
            firstName
            lastName
          }
        }
        lastMessage {
          sender{
            username
            firstName
            lastName
          }
          time
          message
        }
      }
    }
  }
`;

export const GET_CHAT_MESSAGES_CHUNK_BY_OFFSET = gql`
query GetChatById($chatId: String!, $messageOffset: Int!, $chatType: ChatType){
    userChatByChatId(chatId: $chatId, chatType: $chatType, take: 1) {
      items {
        messages(skip: $messageOffset) {
          items {
            sender {
              username
              firstName
              lastName
            }
            time
            message
          }
  
          pageInfo {
            hasNextPage
          }
        }
      }
    }
  }
`;

export const GET_FULLCHAT_BY_ID_WITH_MESSAGES_CHUNK = gql`
query GetChatById($chatId: String!, $messageOffset: Int!, $chatType: ChatType){
    userChatByChatId(chatId: $chatId, chatType: $chatType, take: 1) {
      items {
        chatId
        type
        name
        participants {
          items {
            username
            firstName
            lastName
          }
        }
        messages(skip: $messageOffset) {
          items {
            sender {
              username
              firstName
              lastName
            }
            time
            message
          }
          pageInfo {
            hasNextPage
          }
        }
      }
    }
  }
`;

export const SEARCH_USER_BY_NAME = gql`
query SearchUserByName($username: String!, $exact: Boolean!) {
    searchUser(name: $username, exact: $exact) {
      items {
        username
        firstName
        lastName
      }
    }
  }
`;