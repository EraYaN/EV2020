load train;
p = audioplayer(y, Fs);
play(p);
get(p);

column = ceil(length(y)/Fs / 0.02);
row = ceil(length(y)/(length(y)/Fs / 0.02));
X = zeros(row, column);
y = [y;zeros(76,1)];
X = reshape(y, row, column);

Y = fft(X);
t = [0:0.02:length(y)/Fs];
f = [0: 2*pi/row : (row-1)*2*pi/row]-pi;
imagesc(t,f,abs(Y))
xlabel('Time (s)');
ylabel('Frequency (Hz)');