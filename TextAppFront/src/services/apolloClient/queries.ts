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

export const USER_LOGIN = gql`
query Login($username: String!, $password: String!){
    token: login(login: {username: $username, password: $password})
}
`;