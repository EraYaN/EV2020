%% Genereren van de data
clear all
clc;
close all
N=100;
L=4;
h=[3,1,2,-4];
% h=[1 2 3 4 5 6 7 8 9 10];

t=zeros(1,N);
for i=1:N
    t(i)=i;
end

%minimum-phase sequence
x_1 = [1 ; -0.5];
X_1 = [x_1; zeros((N-length(x_1)),1)];

%maximum-phase sequence
x_2 = [1;-2];
X_2 = [x_2; zeros((N-length(x_2)),1)];

%sinusoidal signal
%omega=0.2
omega = 0.2;
X_3 = [zeros(N,1)];
for i = 1:(N-1)
    X_3(i) = cos(omega*i);
end

%random BPSK sequence
X_4 = sign(randn(N,1));

%convolution
y_1 = conv(X_1,h);
y_2 = conv(X_2,h);
y_3 = conv(X_3,h);
y_4 = conv(X_4,h);

%% Singular values aanmaken
Xmat=zeros(N,6);
Xmat(:,1)= X_1;
Xmat(:,2)= X_2;
Xmat(:,3)= X_3;
Xmat(:,4)= X_4;

for i=1:4
Ltest=6;
Lmatrix=zeros(1,10);
figure;
    for j=1:10
        Ltest=Ltest+j-1;
        Lmatrix(j)=Ltest;
        y=conv(Xmat(:,i),h);
        plot(svd(toep(Xmat(:,i),length(y),Ltest)),'Color',[0 j/10 j/10], 'Marker','+')
        hold on;
    end
        legend(['L ', num2str(Lmatrix(1))],['L ', num2str(Lmatrix(2))], ['L ', num2str(Lmatrix(3))], ['L ', num2str(Lmatrix(4))], ['L ', num2str(Lmatrix(5))], ['L ', num2str(Lmatrix(6))], ['L ', num2str(Lmatrix(7))], ['L ', num2str(Lmatrix(8))], ['L ', num2str(Lmatrix(9))], ['L ', num2str(Lmatrix(10))])
        title(['SVD van x',num2str(i)])
end