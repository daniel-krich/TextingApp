import { gql } from "@apollo/client";


export const SUBSCRIBE_LISTEN_MESSAGES = gql`
subscription {
    listenChatUpdates {
      chatId
      type
      lastMessage{
        sender{
          username
          firstName
          lastName
        }
        message
        time
      }
    }
  }
`;