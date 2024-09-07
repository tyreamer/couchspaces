import { initializeApp } from 'https://www.gstatic.com/firebasejs/9.6.1/firebase-app.js';
import { getAuth, GoogleAuthProvider, signInWithPopup } from 'https://www.gstatic.com/firebasejs/9.6.1/firebase-auth.js';

const firebaseConfig = {
    apiKey: 'AIzaSyBsZUY1oKZ4Pt0_yenRbDgTfROE9HaJN3g',
    authDomain: 'couchspaces.firebaseapp.com',
    projectId: 'couchspaces',
    storageBucket: 'couchspaces.appspot.com',
    messagingSenderId: '748748625200',
    appId: '1:748748625200:web:f13fdce29f14453a25a4f5'
};

// Initialize Firebase
const app = initializeApp(firebaseConfig);
const auth = getAuth(app);

window.firebaseAuth = {
    signInWithGoogle: async function () {
        const provider = new GoogleAuthProvider();
        try {
            const result = await signInWithPopup(auth, provider);
            const user = result.user;
            if (user) {
                const token = await user.getIdTokenResult();
                const userData = {
                    uid: user.uid,
                    displayName: user.displayName,
                    photoURL: user.photoURL,
                    email: user.email,
                    emailVerified: user.emailVerified,
                    phoneNumber: user.phoneNumber,
                    isAnonymous: user.isAnonymous,
                    tenantId: user.tenantId,
                    providerData: user.providerData.map(provider => ({
                        uid: provider.uid,
                        displayName: provider.displayName,
                        photoURL: provider.photoURL,
                        email: provider.email,
                        phoneNumber: provider.phoneNumber,
                        providerId: provider.providerId
                    })),
                    apiKey: user.apiKey,
                    appName: user.appName,
                    authDomain: user.authDomain,
                    stsTokenManager: {
                        apiKey: token.claims.apiKey,
                        refreshToken: user.refreshToken,
                        accessToken: token.token,
                        expirationTime: token.expirationTime // Keep as string
                    },
                    lastLoginAt: user.metadata.lastSignInTime,
                    createdAt: user.metadata.creationTime,
                    multiFactor: user.multiFactor ? {
                        enrolledFactors: user.multiFactor.enrolledFactors
                    } : null
                };
                localStorage.setItem('firebaseUser', JSON.stringify(userData));
                startTokenValidationCheck(userData);
                return userData;
            } else {
                console.error("User is null after sign-in");
                return null;
            }
        } catch (error) {
            console.error("Error signing in with Google: ", error);
            return null;
        }
    },

    isTokenValid: function (expirationTime) {
        const currentTime = new Date().toISOString();
        return currentTime < expirationTime;
    },

    refreshToken: async function () {
        try {
            const user = auth.currentUser;
            if (user) {
                const token = await user.getIdToken(true); // Force refresh
                const tokenResult = await user.getIdTokenResult();
                return {
                    accessToken: token,
                    expirationTime: tokenResult.expirationTime
                };
            } else {
                console.error("Current user is null");
                return null;
            }
        } catch (error) {
            console.error("Error refreshing token: ", error);
            return null;
        }
    }
};

function startTokenValidationCheck(user) {
    setInterval(async () => {
        if (user && user.stsTokenManager) {
            const tokenResult = await auth.currentUser.getIdTokenResult();
            if (!window.firebaseAuth.isTokenValid(tokenResult.expirationTime)) {
                const refreshedToken = await window.firebaseAuth.refreshToken();
                if (refreshedToken) {
                    user.stsTokenManager.accessToken = refreshedToken.accessToken;
                    user.stsTokenManager.expirationTime = refreshedToken.expirationTime;
                    updateUI(user);
                } else {
                    console.error("Failed to refresh token");
                }
            }
        } else {
            console.error("User or stsTokenManager is null during token validation check");
        }
    }, 5 * 60 * 1000); // Check every 5 minutes
}

window.updateUI = function (user) {
    document.getElementById('login-button').style.display = 'none';
    document.getElementById('user-info').style.display = 'block';
    document.getElementById('user-name').innerText = `Name: ${user.displayName}`;
    document.getElementById('user-email').innerText = `Email: ${user.email}`;
    document.getElementById('user-token').innerText = `Token: ${user.stsTokenManager.accessToken}`;
};

// Check authentication state on page load
window.addEventListener('load', async () => {
    const storedUser = localStorage.getItem('firebaseUser');
    if (storedUser) {
        const user = JSON.parse(storedUser);
        if (window.firebaseAuth.isTokenValid(user.stsTokenManager.expirationTime)) {
            updateUI(user);
            startTokenValidationCheck(user);
        } else {
            const refreshedToken = await window.firebaseAuth.refreshToken();
            if (refreshedToken) {
                user.stsTokenManager.accessToken = refreshedToken.accessToken;
                user.stsTokenManager.expirationTime = refreshedToken.expirationTime;
                localStorage.setItem('firebaseUser', JSON.stringify(user));
                updateUI(user);
                startTokenValidationCheck(user);
            } else {
                console.error("Failed to refresh token on page load");
            }
        }
    }
});
