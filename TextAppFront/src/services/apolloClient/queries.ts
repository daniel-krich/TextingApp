import { gql } from "@apollo/client";

export const USER_INFO = gql`
query GetUser {
    user {
      username
      firstName
      lastName
      email
    }
  }
`;