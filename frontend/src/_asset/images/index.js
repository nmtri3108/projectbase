const images = {
    noAvatar: import('./noAva.png').then((module) => module.default),
    background: import('./background.jpg').then((module) => module.default),
};

export default images;