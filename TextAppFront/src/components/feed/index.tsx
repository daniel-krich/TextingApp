import React, { useEffect, useState } from 'react';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react';
import { useGlobalStore } from '../../services';
import { useNavigate, Navigate } from "react-router-dom";
import { Container, Row, Col } from 'react-bootstrap';
import 'bootstrap/dist/css/bootstrap.min.css';
import { Post } from './post';
import { Publish } from './publish';

interface PostStruct {
    creator: string,
    text: string,
    date: Date
}

export const Feed = observer(() => {
    const [posts, setPosts] = useState({items: [] as PostStruct[]});
    const globalcontext = useGlobalStore();
    const authService = globalcontext.authService;
    useEffect(() => {
        const newPosts = {items: [] as PostStruct[]};
        newPosts.items.push({creator: "Dummy", text: "hello world! this is a dummy post...\n...\n...\nabcd.", date: new Date('Wed, 11 Mar 2022 08:10:50 GMT')});
        setPosts(newPosts);
    }, []);
    return (
        <Container className='feed-container'>
            <Publish/>
            {
                posts.items.map(p => 
                    <Post text={p.text} creator={p.creator} date={p.date}/>
                )
            }
            
        </Container>
)});