import { initializeApp } from 'https://www.gstatic.com/firebasejs/9.6.1/firebase-app.js';
import { getAuth, GoogleAuthProvider, signInWithPopup, signOut, onAuthStateChanged } from 'https://www.gstatic.com/firebasejs/9.6.1/firebase-auth.js';

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

console.log("Firebase initialized");

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
                    appName: user.appName,
                    authDomain: user.authDomain,
                    stsTokenManager: {
                        refreshToken: user.refreshToken,
                        accessToken: token.token,
                        expirationTime: token.expirationTime
                    },
                    lastLoginAt: user.metadata.lastSignInTime,
                    createdAt: user.metadata.creationTime,
                    multiFactor: user.multiFactor ? {
                        enrolledFactors: user.multiFactor.enrolledFactors
                    } : null
                };
                localStorage.setItem('couchspacesUser', JSON.stringify(userData));
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

    signOut: async function () {
        try {
            await signOut(auth);
            localStorage.removeItem('couchspacesUser');
        } catch (error) {
            console.error("Error signing out: ", error);
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
                    expirationTime: tokenResult.expirationTime,
                    refreshToken: tokenResult.refreshToken
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
            try {
                const tokenResult = await auth.currentUser.getIdTokenResult();
                if (!window.firebaseAuth.isTokenValid(tokenResult.expirationTime)) {
                    const refreshedToken = await window.firebaseAuth.refreshToken();
                    if (refreshedToken) {
                        user.stsTokenManager.accessToken = refreshedToken.accessToken;
                        user.stsTokenManager.expirationTime = refreshedToken.expirationTime;
                        user.stsTokenManager.refreshToken = refreshedToken.refreshToken;
                        localStorage.setItem('couchspacesUser', JSON.stringify(user)); // Update localStorage
                    } else {
                        console.error("Failed to refresh token");
                    }
                }
            } catch (error) {
                console.error("Error during token validation check: ", error);
            }
        } else {
            console.error("User or stsTokenManager is null during token validation check");
        }
    }, 5 * 60 * 1000); // Check every 5 minutes
}

window.addEventListener('load', () => {
    console.log("Page loaded, setting up auth state listener");

    onAuthStateChanged(auth, async (user) => {
        if (user) {
            const storedUser = localStorage.getItem('couchspacesUser');

            if (storedUser) {
                let userData;
                try {
                    userData = JSON.parse(storedUser);

                    // Check if StsTokenManager is defined
                    if (!userData.StsTokenManager) {
                        console.error("StsTokenManager is undefined in userData");
                        return;
                    }

                } catch (e) {
                    console.error("Error parsing stored user data:", e);
                    await window.firebaseAuth.signOut();
                    return;
                }
                const expirationTime = new Date(userData.StsTokenManager.ExpirationTime);

                if (window.firebaseAuth.isTokenValid(expirationTime)) {
                    startTokenValidationCheck(userData);
                }
                else {
                    const refreshedToken = await window.firebaseAuth.refreshToken();
                    if (refreshedToken) {
                        userData.StsTokenManager.AccessToken = refreshedToken.accessToken;
                        userData.StsTokenManager.ExpirationTime = refreshedToken.expirationTime;
                        userData.StsTokenManager.RefreshToken = refreshedToken.refreshToken;
                        localStorage.setItem('couchspacesUser', JSON.stringify(userData));
                        startTokenValidationCheck(userData);
                    }
                    else {
                        console.error("Failed to refresh token on page load");
                        await window.firebaseAuth.signOut();
                    }
                }
            }
        }
        else {
            console.error("No user is signed in");
        }
    });
});

window.firebaseAuth.isTokenValid = function (expirationTime) {
    const currentTime = new Date();
    console.log("Current time:", currentTime);
    console.log("Expiration time:", expirationTime);
    return currentTime < expirationTime;
};
