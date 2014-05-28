close all;
clear all;
clc

N = 100;              % start with small value
h = [3, 1, 2, -4];
%h = [1 2 3 4 5 6 7 8 9 10];
L = 4; 

t = zeros(1,N);
for i = 1:N
    t(i) =i;
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
%y_1 = conv(X_1,h);
%y_2 = conv(X_2,h);
%y_3 = conv(X_3,h);
%y_4 = conv(X_4,h);

Xmat=[X_1, X_2,X_3,X_4];

%convolution
for i=1:4
    Ymat(:,i)=conv(Xmat(:,i),h);
end



%Gaussian noise 
Ny=length(Ymat(:,1));
sigma=0.01;
noise= sigma*randn(Ny,1);

for i=1:4
    Ymat(:,i)=Ymat(:,i)+noise;
end




%ch1
hold on;
for i=1:64
    for L=1:50
        z_1 = ch1(Xmat(:,i),Ymat(:,i),L);
        [hhat, error] = error_criterion(h,z_1);
        error1(L,i) = error;
    end
    
end
plot (error1)
title('Equalizer 1')
xlabel('L')
ylabel('Error')
legend('x_1', 'x_2','x_3','x_4')

%ch2
figure;
hold on
for i=1:4
    for L=1:50
        z_2 = ch2(Xmat(:,i),Ymat(:,i),L);
        [hhat,error]=error_criterion(h,z_2);
        error2(L,i)=error;
    end
end
plot (error2);
title('Equalizer 2')
xlabel('L')
ylabel('Error')
legend('x_1', 'x_2','x_3','x_4')