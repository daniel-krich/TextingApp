import { gql } from "@apollo/client";

export const REGISTER = gql`
mutation Register($username: String!,
    $password: String!,
    $email: String!,
    $firstName: String!,
    $lastName: String!){
user: createUser(createUser: {username: $username,
            password: $password,
            email: $email,
            firstName: $firstName,
            lastName: $lastName}) {
username
firstName
lastName
}
}
`;

export const SEND_MESSAGE = gql`
mutation SendMessage($chatId: String!, $chatType: ChatType!, $message: String!){
    addMessageToChat(message: {chatId: $chatId, typeChat: $chatType, message: $message}) {
      lastMessage {
        message
      }
    }
  }
`;