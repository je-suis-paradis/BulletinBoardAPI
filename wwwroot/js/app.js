let token = localStorage.getItem('token');
let currentUser = JSON.parse(localStorage.getItem('user'));
let currentPostId = null;

const API_URL = 'http://localhost:5239/api';

const authView = document.getElementById('auth-view');
const postsView = document.getElementById('posts-view');
const singlePostView = document.getElementById('single-post-view');

const userInfo = document.getElementById('user-info');
const logoutBtn = document.getElementById('logout-btn');

const loginEmail = document.getElementById('login-email');
const loginPassword = document.getElementById('login-password');
const loginBtn = document.getElementById('login-btn');

const registerName = document.getElementById('register-name');
const registerHandle = document.getElementById('register-handle');
const registerEmail = document.getElementById('register-email');
const registerPassword = document.getElementById('register-password');
const registerBtn = document.getElementById('register-btn');

const postTitle = document.getElementById('post-title');
const postText = document.getElementById('post-text');
const postCategory = document.getElementById('post-category');
const createPostBtn = document.getElementById('create-post-btn');
const postsContainer = document.getElementById('posts-container');

const backToPostsBtn = document.getElementById('back-to-posts');
const singlePostContainer = document.getElementById('single-post-container');
const responsesContainer = document.getElementById('responses-container');
const responseText = document.getElementById('response-text');
const addResponseBtn = document.getElementById('add-response-btn');

function showView(view) {
    authView.hidden = true;
    postsView.hidden = true;
    singlePostView.hidden = true;
    view.hidden = false;
}

function updateAuthUI() {
    if (token && currentUser) {
        userInfo.textContent = `Welcome, @${currentUser.handle}!`;
        logoutBtn.hidden = false;
        showView(postsView);
        loadPosts();
    } else {
        token = null;
        currentUser = null;
        localStorage.removeItem('token');
        localStorage.removeItem('user');

        userInfo.textContent = '';
        logoutBtn.hidden = true;
        showView(authView);
    }
}

async function apiCall(endpoint, method = 'GET', body = null) {
    const headers = {
        'Content-Type': 'application/json'
    };

    if (token) {
        headers['Authorization'] = `Bearer ${token}`;
    }

    const options = {
        method,
        headers
    };

    if (body) {
        options.body = JSON.stringify(body);
    }

    const response = await fetch(`${API_URL}${endpoint}`, options);

    if (!response.ok) {
        const error = await response.text();
        throw new Error(error || 'Something went wrong');
    }

    const text = await response.text();
    return text ? JSON.parse(text) : null;
}

async function login() {
    try {
        const data = await apiCall('/Auth/login', 'POST', {
            email: loginEmail.value,
            password: loginPassword.value
        });

        token = data.token;
        currentUser = {
            email: data.email,
            handle: data.handle,
            name: data.name
        };

        localStorage.setItem('token', token);
        localStorage.setItem('user', JSON.stringify(currentUser));

        updateAuthUI();
    } catch (error) {
        alert('Login failed: ' + error.message);
    }
}

async function register() {
    try {
        const data = await apiCall('/Auth/register', 'POST', {
            name: registerName.value,
            handle: registerHandle.value,
            email: registerEmail.value,
            password: registerPassword.value
        });

        token = data.token;
        currentUser = {
            email: data.email,
            handle: data.handle,
            name: data.name
        };

        localStorage.setItem('token', token);
        localStorage.setItem('user', JSON.stringify(currentUser));

        updateAuthUI();
    } catch (error) {
        alert('Registration failed: ' + error.message);
    }
}

function logout() {
    token = null;
    currentUser = null;
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    updateAuthUI();
}

async function loadPosts() {
    try {
        const posts = await apiCall('/Posts');
        displayPosts(posts);
    } catch (error) {
        console.error('Failed to load posts:', error);
    }
}

function displayPosts(posts) {
    postsContainer.innerHTML = '';

    if (posts.length === 0) {
        postsContainer.innerHTML = '<p>No posts yet. Be the first to post!</p>';
        return;
    }

    posts.forEach(post => {
        const postCard = document.createElement('div');
        postCard.className = 'post-card';
        postCard.innerHTML = `
            <h3>${post.title}</h3>
            <div class="post-meta">
                Posted by @${post.author?.handle || 'unknown'}
            </div>
            <span class="post-category">${post.category}</span>
            <p class="post-text">${post.text.substring(0, 150)}${post.text.length > 150 ? '...' : ''}</p>
        `;
        postCard.addEventListener('click', () => viewPost(post.id));
        postsContainer.appendChild(postCard);
    });
}

async function createPost() {
    try {
        await apiCall('/Posts', 'POST', {
            title: postTitle.value,
            text: postText.value,
            category: postCategory.value
        });

        postTitle.value = '';
        postText.value = '';
        postCategory.value = '';

        loadPosts();
    } catch (error) {
        alert('Failed to create post: ' + error.message);
    }
}

async function viewPost(postId) {
    currentPostId = postId;

    try {
        const post = await apiCall(`/Posts/${postId}`);
        displaySinglePost(post);
        showView(singlePostView);
    } catch (error) {
        alert('Failed to load post: ' + error.message);
    }
}

function displaySinglePost(post) {
    singlePostContainer.innerHTML = `
        <div class="post-card">
            <h3>${post.title}</h3>
            <div class="post-meta">
                Posted by @${post.author?.handle || 'unknown'}
            </div>
            <span class="post-category">${post.category}</span>
            <p class="post-text">${post.text}</p>
        </div>
    `;

    displayResponses(post.responses || []);
}

function displayResponses(responses) {
    responsesContainer.innerHTML = '';

    if (responses.length === 0) {
        responsesContainer.innerHTML = '<p>No responses yet.</p>';
        return;
    }

    responses.forEach(response => {
        const responseCard = document.createElement('div');
        responseCard.className = 'response-card';
        responseCard.innerHTML = `
            <div class="response-meta">
                @${response.author?.handle || 'unknown'}
            </div>
            <p>${response.text}</p>
        `;
        responsesContainer.appendChild(responseCard);
    });
}

async function addResponse() {
    try {
        await apiCall('/Responses', 'POST', {
            text: responseText.value,
            postId: currentPostId
        });

        responseText.value = '';

        viewPost(currentPostId);
    } catch (error) {
        alert('Failed to add response: ' + error.message);
    }
}

loginBtn.addEventListener('click', login);
registerBtn.addEventListener('click', register);
logoutBtn.addEventListener('click', logout);
createPostBtn.addEventListener('click', createPost);
backToPostsBtn.addEventListener('click', () => {
    showView(postsView);
    loadPosts();
});
addResponseBtn.addEventListener('click', addResponse);

updateAuthUI();